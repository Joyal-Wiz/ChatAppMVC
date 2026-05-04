using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using ChatAppMVC.DTOs;
using ChatAppMVC.Helpers;
using ChatAppMVC.Models;
using ChatAppMVC.Repository.Interfaces;
using ChatAppMVC.Services.Interfaces;

namespace ChatAppMVC.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IOnlineUserTracker _onlineUserTracker;
        private readonly JwtHelper _jwtHelper;
        private readonly IWebHostEnvironment _environment;

        public UserService(IUserRepository userRepository, 
            IMessageRepository messageRepository, 
            IOnlineUserTracker onlineUserTracker, 
            JwtHelper jwtHelper,
            IWebHostEnvironment environment)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _onlineUserTracker = onlineUserTracker;
            _jwtHelper = jwtHelper;
            _environment = environment;
        }

        public async Task<ApiResponse<UserResponseDto>> RegisterAsync(RegisterUserDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email ?? "");

            if (existingUser != null)
            {
                return new ApiResponse<UserResponseDto>(
                    false,
                    "User already exists",
                    null,
                    400
                );
            }

            string profilePicUrl = "/uploads/default-avatar.png";
            if (dto.ProfilePicture != null)
            {
                profilePicUrl = await SaveFile(dto.ProfilePicture);
            }

            var user = new User
            {
                Username = dto.Username ?? "",
                Email = dto.Email ?? "",
                Password = PasswordHasher.Hash(dto.Password ?? ""),
                ProfilePictureUrl = profilePicUrl
            };

            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            var response = new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl
            };

            return new ApiResponse<UserResponseDto>(
                true,
                "User registered successfully",
                response,
                201
            );
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/" + fileName;
        }

        public async Task<ApiResponse<List<UserResponseDto>>> GetAllUsersAsync(int currentUserId)
        {
            var users = await _userRepository.GetAllUsersAsync();
            var response = new List<UserResponseDto>();

            foreach (var u in users)
            {
                if (u.Id == currentUserId) continue;

                var lastMsg = await _messageRepository.GetLastMessageAsync(currentUserId, u.Id);

                response.Add(new UserResponseDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    IsOnline = _onlineUserTracker.IsUserOnline(u.Id),
                    LastMessage = lastMsg?.Content ?? "Click to start chatting",
                    LastMessageTime = lastMsg?.SentAt,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    Status = u.Status
                });
            }

            return new ApiResponse<List<UserResponseDto>>(
                true,
                "Users fetched",
                response,
                200
            );
        }

        public async Task<ApiResponse<object>> LoginAsync(LoginUserDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrEmpty(dto.Email))
                {
                    return new ApiResponse<object>(false, "Email and Password are required", null, 400);
                }

                var user = await _userRepository.GetByEmailAsync(dto.Email);

                if (user == null || !PasswordHasher.Verify(dto.Password ?? "", user.Password))
                {
                    return new ApiResponse<object>(
                        false,
                        "Invalid credentials",
                        null,
                        401
                    );
                }

                // 🔥 Generate JWT Token
                var token = _jwtHelper.GenerateToken(user);

                return new ApiResponse<object>(
                    true,
                    "Login successful",
                    new
                    {
                        user.Id,
                        user.Username,
                        user.Email,
                        Token = token,
                        ProfilePictureUrl = user.ProfilePictureUrl
                    },
                    200
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LOGIN ERROR: {ex}");
                throw; // Rethrow to let middleware handle JSON response
            }
        }

        public async Task<ApiResponse<string>> UpdateStatusAsync(int userId, string status)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return new ApiResponse<string>(false, "User not found", null, 404);

            user.Status = status;
            await _userRepository.SaveChangesAsync();

            return new ApiResponse<string>(true, "Status updated", null, 200);
        }

        public async Task<ApiResponse<string>> UpdateProfileAsync(int userId, string status, IFormFile? profilePicture)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return new ApiResponse<string>(false, "User not found", null, 404);

            if (!string.IsNullOrEmpty(status)) user.Status = status;
            
            if (profilePicture != null)
            {
                user.ProfilePictureUrl = await SaveFile(profilePicture);
            }

            await _userRepository.SaveChangesAsync();

            return new ApiResponse<string>(true, "Profile updated", user.ProfilePictureUrl, 200);
        }
    }
}
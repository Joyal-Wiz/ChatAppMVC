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

        public UserService(IUserRepository userRepository, 
            IMessageRepository messageRepository, 
            IOnlineUserTracker onlineUserTracker, 
            JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _onlineUserTracker = onlineUserTracker;
            _jwtHelper = jwtHelper;
        }

        public async Task<ApiResponse<UserResponseDto>> RegisterAsync(RegisterUserDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                return new ApiResponse<UserResponseDto>(
                    false,
                    "User already exists",
                    null,
                    400
                );
            }

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = PasswordHasher.Hash(dto.Password)
            };

            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            var response = new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };

            return new ApiResponse<UserResponseDto>(
                true,
                "User registered successfully",
                response,
                201
            );
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
                    LastMessageTime = lastMsg?.SentAt
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
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null || !PasswordHasher.Verify(dto.Password, user.Password))
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
                    Token = token
                },
                200
            );
        }
    }
}
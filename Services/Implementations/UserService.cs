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
        private readonly JwtHelper _jwtHelper;

        public UserService(IUserRepository userRepository, JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
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
        public async Task<ApiResponse<List<UserResponseDto>>> GetAllUsersAsync()
{
    var users = await _userRepository.GetAllUsersAsync();

    var response = users.Select(u => new UserResponseDto
    {
        Id = u.Id,
        Username = u.Username,
        Email = u.Email
    }).ToList();

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
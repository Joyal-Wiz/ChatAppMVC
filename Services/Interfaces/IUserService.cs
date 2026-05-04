using ChatAppMVC.DTOs;
using ChatAppMVC.Helpers;

namespace ChatAppMVC.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<UserResponseDto>> RegisterAsync(RegisterUserDto dto);

        Task<ApiResponse<object>> LoginAsync(LoginUserDto dto);
        Task<ApiResponse<List<UserResponseDto>>> GetAllUsersAsync(int currentUserId);
    }
}
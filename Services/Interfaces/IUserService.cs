using Microsoft.AspNetCore.Http;
using ChatAppMVC.DTOs;
using ChatAppMVC.Helpers;

namespace ChatAppMVC.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<UserResponseDto>> RegisterAsync(RegisterUserDto dto);

        Task<ApiResponse<object>> LoginAsync(LoginUserDto dto);
        Task<ApiResponse<List<UserResponseDto>>> GetAllUsersAsync(int currentUserId);
        Task<ApiResponse<string>> UpdateStatusAsync(int userId, string status);
        Task<ApiResponse<string>> UpdateProfileAsync(int userId, string status, IFormFile? profilePicture);
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using ChatAppMVC.Services.Interfaces;

[Authorize]
[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var currentUserId = int.Parse(User.FindFirst("UserId").Value);
        var result = await _userService.GetAllUsersAsync(currentUserId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("status")]
    public async Task<IActionResult> UpdateStatus([FromBody] string status)
    {
        var currentUserId = int.Parse(User.FindFirst("UserId").Value);
        var result = await _userService.UpdateStatusAsync(currentUserId, status);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromForm] string status, [FromForm] IFormFile? profilePicture)
    {
        var currentUserId = int.Parse(User.FindFirst("UserId").Value);
        var result = await _userService.UpdateProfileAsync(currentUserId, status, profilePicture);
        return StatusCode(result.StatusCode, result);
    }
}
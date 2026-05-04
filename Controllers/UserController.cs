using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
}
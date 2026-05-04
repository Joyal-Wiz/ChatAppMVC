using Microsoft.AspNetCore.Mvc;
using ChatAppMVC.Services.Interfaces;
using ChatAppMVC.DTOs;

namespace ChatAppMVC.Controllers
{
    [ApiController] // 🔥 enables JSON binding
    [Route("")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        // ✅ Load Login Page
        [HttpGet("Auth/Login")]
        public IActionResult LoginPage()
        {
            return View("Login");
        }

        // ✅ Load Register Page
        [HttpGet("Auth/Register")]
        public IActionResult RegisterPage()
        {
            return View("Register");
        }

        // ✅ API: Register (returns JSON)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            var result = await _userService.RegisterAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        // ✅ API: Login (returns JSON + JWT)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            var result = await _userService.LoginAsync(dto);
            return StatusCode(result.StatusCode, result);
        }
    }
}
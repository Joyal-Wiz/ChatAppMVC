using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ChatAppMVC.Services.Interfaces;
using ChatAppMVC.DTOs;

namespace ChatAppMVC.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/messages")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessageDto dto)
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);

            var result = await _messageService.SendMessageAsync(userId, dto);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetMessages(int userId)
        {
            var currentUserId = int.Parse(User.FindFirst("UserId").Value);

            var result = await _messageService.GetMessagesAsync(currentUserId, userId);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("read/{userId}")]
        public async Task<IActionResult> MarkAsRead(int userId)
        {
            var currentUserId = int.Parse(User.FindFirst("UserId").Value);

            var result = await _messageService.MarkMessagesAsReadAsync(currentUserId, userId);

            return StatusCode(result.StatusCode, result);
        }
    }
}
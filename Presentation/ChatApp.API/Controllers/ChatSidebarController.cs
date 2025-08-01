using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [ApiController]
    [Route("api/chatSidebar")]
    [Authorize]
    public class ChatSidebarController : ControllerBase
    {
        private readonly IChatSideBarService _chatSideBarService;
        public ChatSidebarController(IChatSideBarService chatSideBarService)
        {
            _chatSideBarService = chatSideBarService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserGroupChats()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var chats = await _chatSideBarService.GetUserGroupChatsWithParticipantsAsync(int.Parse(userId));
            return Ok(chats);
        }

        [HttpGet("simple")]
        public async Task<IActionResult> GetUserGroupChatsSimple()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var chats = await _chatSideBarService.GetUserGroupChatsAsync(int.Parse(userId));
            return Ok(chats);
        }
    }
}
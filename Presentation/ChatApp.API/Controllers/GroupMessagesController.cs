using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using ChatApp.Application.DTOs.MessageDTOs;
using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [ApiController]
    [Route("api/groups")]
    [Authorize]
    public class GroupMessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public GroupMessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        // GET: api/groups/{groupId}/messages
        [HttpGet("{groupId:int}/messages")]
        public async Task<IActionResult> GetMessages(int groupId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int currentUserId))
                {
                    return Unauthorized("Geçersiz kullanıcı bilgisi.");
                }

                var messages = await _messageService.GetMessagesByGroupIdAsync(groupId, currentUserId);
                
                return Ok(messages);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
 
        }

        // POST: api/groups/{groupId}/messages
        [HttpPost("{groupId:int}/messages")]
        public async Task<IActionResult> SendMessage(int groupId, [FromBody] CreateMessageDto messageDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int senderId))
                {
                    return Unauthorized("Geçersiz kullanıcı bilgisi.");
                }

                var message = await _messageService.CreateMessageAsync(senderId, groupId, messageDto.Content);
                return Ok(new { message = "Mesaj başarıyla gönderildi.", messageId = message.MessageId });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // PUT: api/groups/{groupId}/messages/{messageId}
        [HttpPut("{groupId:int}/messages/{messageId:int}")]
        public async Task<IActionResult> UpdateMessage(int groupId, int messageId, [FromBody] CreateMessageDto messageDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _messageService.UpdateMessageAsync(messageId, messageDto.Content);
                return Ok(new { message = "Mesaj başarıyla güncellendi." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("{groupId:int}/messages/{messageId:int}")]
        public async Task<IActionResult> DeleteMessage(int groupId, int messageId)
        {
            try
            {
                await _messageService.DeleteMessageAsync(messageId);
                return Ok(new { message = "Mesaj başarıyla silindi." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
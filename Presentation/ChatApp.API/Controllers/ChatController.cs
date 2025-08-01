using ChatApp.Application.DTOs.GroupDTOs;
using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;
        private readonly IGroupUserService _groupUserService;

        public ChatController(IUserService userService, IGroupService groupService, IGroupUserService groupUserService)
        {
            _userService = userService;
            _groupService = groupService;
            _groupUserService = groupUserService;
        }

        [HttpGet("search-users")]
        
        public async Task<IActionResult> SearchUsers([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return Ok(new List<object>());
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int currentUserId))
                {
                    return Unauthorized("Geçersiz kullanıcı bilgisi.");
                }

                var users = await _userService.SearchUsersByUsernameAsync(searchTerm);

                var filteredUsers = users.Where(u => u.UserId != currentUserId);

                var result = filteredUsers.Select(u => new
                {
                    userId = u.UserId,
                    userName = u.UserName,
                    name = u.Name,
                    lastName = u.LastName,
                    displayName = $"{u.Name} {u.LastName} (@{u.UserName})"
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Kullanıcı arama sırasında bir hata oluştu.");
            }
        }

       
        [HttpGet("group/{groupId}/details")]
        public async Task<IActionResult> GetGroupDetails(int groupId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int currentUserId))
                {
                    return Unauthorized("Geçersiz kullanıcı bilgisi.");
                }

                var group = await _groupService.GetGroupByIdAsync(groupId);
                if (group == null)
                {
                    return NotFound("Grup bulunamadı.");
                }

                var groupUsers = await _groupService.GetGroupUsersAsync(groupId);
                var members = new List<object>();
                string displayGroupName = group.GroupName;

                foreach (var groupUser in groupUsers)
                {
                    var user = await _userService.GetUserByIdAsync(groupUser.UserId);
                    if (user != null)
                    {
                        members.Add(new
                        {
                            userId = user.UserId,
                            userName = user.UserName,
                            displayName = $"{user.Name} {user.LastName}",
                            isOwner = user.UserId == group.CreatedByUserId
                        });
                    }
                }

                if (members.Count == 2)
                {
                    var otherMember = members.Cast<dynamic>().FirstOrDefault(m => m.userId != currentUserId);
                    if (otherMember != null)
                    {
                        displayGroupName = otherMember.displayName;
                    }
                }

                var createdByUser = await _userService.GetUserByIdAsync(group.CreatedByUserId);
                string createdByName = createdByUser != null ? $"{createdByUser.Name} {createdByUser.LastName}" : "Bilinmeyen";

                return Ok(new 
                { 
                    groupId = group.GroupId,
                    groupName = displayGroupName,
                    originalGroupName = group.GroupName,
                    createdByName = createdByName,
                    createdDate = group.CreatedAt,
                    isCurrentUserOwner = group.CreatedByUserId == currentUserId,
                    isPrivateChat = members.Count == 2,
                    members = members
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Grup detayları alınırken bir hata oluştu.");
            }
        }

        // [HttpPut("group/{groupId}/name")]
        // public async Task<IActionResult> UpdateGroupName(int groupId, [FromBody] UpdateGroupNameRequest request)
        // {
        //     try
        //     {
        //         var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //         if (!int.TryParse(userIdClaim, out int currentUserId))
        //         {
        //             return Unauthorized("Geçersiz kullanıcı bilgisi.");
        //         }

        //         var updateDto = new UpdateGroupDto
        //         {
        //             GroupId = groupId,
        //             Name = request.NewName
        //         };
                
        //         await _groupService.UpdateGroupAsync(updateDto);

        //         return Ok(new 
        //         { 
        //             message = "Grup adı başarıyla güncellendi.",
        //             newName = request.NewName
        //         });
        //     }
        //     catch (KeyNotFoundException)
        //     {
        //         return NotFound("Grup bulunamadı.");
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, "Grup adı güncellenirken bir hata oluştu.");
        //     }
        // }

        [HttpPost("add-user-to-group/{groupId}/{userId}")]
        public async Task<IActionResult> AddUserToGroup(int groupId, int userId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int currentUserId))
                {
                    return Unauthorized("Geçersiz kullanıcı bilgisi.");
                }

                var group = await _groupService.GetGroupByIdAsync(groupId);
                if (group == null)
                {
                    return NotFound("Grup bulunamadı.");
                }

                if (group.CreatedByUserId != currentUserId)
                {
                    return Forbid("Bu işlem için yetkiniz yok. Sadece grup yöneticisi üye ekleyebilir.");
                }

                var userToAdd = await _userService.GetUserByIdAsync(userId);
                if (userToAdd == null)
                {
                    return NotFound("Eklenecek kullanıcı bulunamadı.");
                }

                var existingGroupUsers = await _groupService.GetGroupUsersAsync(groupId);
                if (existingGroupUsers.Any(gu => gu.UserId == userId))
                {
                    return BadRequest("Kullanıcı zaten bu grupta mevcut.");
                }

                await _groupUserService.AddUserToGroupAsync(userId, groupId);

                return Ok(new { message = "Kullanıcı başarıyla gruba eklendi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Kullanıcı gruba eklenirken bir hata oluştu.");
            }
        }

        [HttpDelete("remove-user-from-group/{groupId}/{userId}")]
        public async Task<IActionResult> RemoveUserFromGroup(int groupId, int userId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int currentUserId))
                {
                    return Unauthorized("Geçersiz kullanıcı bilgisi.");
                }

                var group = await _groupService.GetGroupByIdAsync(groupId);
                if (group == null)
                {
                    return NotFound("Grup bulunamadı.");
                }

                if (group.CreatedByUserId != currentUserId)
                {
                    return Forbid("Bu işlem için yetkiniz yok. Sadece grup yöneticisi üye çıkarabilir.");
                }

                if (userId == currentUserId)
                {
                    return BadRequest("Grup yöneticisi kendisini gruptan çıkaramaz.");
                }

                await _groupUserService.RemoveUserFromGroupAsync(userId, groupId);

                return Ok(new { message = "Kullanıcı başarıyla gruptan çıkarıldı." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Kullanıcı gruptan çıkarılırken bir hata oluştu.");
            }
        }
    }

}

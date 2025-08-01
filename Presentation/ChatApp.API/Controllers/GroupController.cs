using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatApp.Application.DTOs.GroupDTOs;
using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : ControllerBase
    {

        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        //Kullanıcı gruplarını listelemek için bir buton koyarsan bu metodu kullanırsın
        [HttpGet]
        public async Task<IActionResult> GetAllGroups()
        {
            var groups = await _groupService.GetAllGroupsAsync();
            return Ok(groups);
        }

        // [HttpGet("by-name/{name}")]
        // public async Task<IActionResult> GetGroupByName(string name)
        // {

        //     var group = await _groupService.GetGroupByNameAsync(name);
        //     return group == null ? NotFound($"Group with name '{name}' not found.") : Ok(group);

        // }



        [HttpPost("create-group")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int creatorUserId))
                {
                    return Unauthorized("Geçersiz kullanıcı bilgisi.");
                }

                var allUserIds = new List<int>(request.UserIds);
                if (!allUserIds.Contains(creatorUserId))
                {
                    allUserIds.Add(creatorUserId);
                }

                var group = await _groupService.CreateGroupWithUsersAsync(
                    request.GroupName, 
                    allUserIds,
                    creatorUserId);

                return Ok(new 
                { 
                    groupId = group.GroupId,
                    groupName = group.GroupName,
                    message = "Grup başarıyla oluşturuldu veya mevcut grup bulundu."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "Grup oluşturulurken bir hata oluştu.");
            }
        }



        // [HttpDelete("{groupId:int}")]
        // public async Task<IActionResult> DeleteGroup(int groupId)
        // {

        // await _groupService.DeleteGroupAsync(groupId);
        // return Ok("Group deleted successfully.");
        // }

        
        [HttpPut("{groupId:int}")]
        public async Task<IActionResult> UpdateGroup(int id, [FromBody] UpdateGroupDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Name))
            {
                return BadRequest("Group data is required.");
            }

            dto.GroupId = id;
            await _groupService.UpdateGroupAsync(dto);
            return Ok("Group updated successfully.");
            
        }





    }
}
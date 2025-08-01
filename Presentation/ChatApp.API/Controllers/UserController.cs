using ChatApp.Application.DTOs.UserDTOs;
using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ChatApp.Application.Services;

namespace ChatApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // [HttpGet("{id}")]
        // public async Task<IActionResult> GetById(int id)
        // {
        //     var user = await _userService.GetUserByIdAsync(id);
        //     return user == null ? NotFound() : Ok(user);
        // }
        

        [HttpGet("by-username/{username}")]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            return user == null ? NotFound() : Ok(user);
        }

        // GET: api/User
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            await _userService.CreateUserAsync(dto);
            return StatusCode(201); // Created
        }

        // [HttpPut]
        // public async Task<IActionResult> Update([FromBody] UpdateUserDto dto)
        // {
        //     await _userService.UpdateUserAsync(dto);
        //     return NoContent();
        // }

        [HttpPut("by-username/{username}")]
        public async Task<IActionResult> UpdateByUsername(string username, [FromBody] UpdateUserByUsernameDto dto)
        {
            try
            {
                await _userService.UpdateUserByUsernameAsync(username, dto);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return NotFound($"User with username '{username}' not found.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update user: {ex.Message}");
            }
        }

        
        [HttpDelete("by-username/{username}")]
        public async Task<IActionResult> DeleteByUsername(string username)
        {
            await _userService.DeleteUserByUsernameAsync(username);
            return NoContent();
        }

        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteById(int id)
        // {
        //     await _userService.DeleteUserByIdAsync(id);
        //     return NoContent();
        // }
    }
}
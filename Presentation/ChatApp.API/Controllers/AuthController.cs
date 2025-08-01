using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApp.Application.DTOs.AuthenticationDTOs;
using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            if (token == null)
                return Unauthorized("Geçersiz kullanıcı adı veya şifre");

            return Ok(new { token });
        }
        // [Authorize]
        // [HttpGet("profile")]
        //  public async Task<IActionResult> GetProfile()
        // {
        // var userName = User.Identity.Name;
        // return Ok(new { userName });
        // }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result)
                return BadRequest("Kullanıcı adı zaten mevcut.");
            return Ok("Kayıt başarılı.");
        }
    }

}
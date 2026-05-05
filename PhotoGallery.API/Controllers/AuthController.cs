using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhotoGallery.Core.Application.DTOs.Auth;
using PhotoGallery.Core.Application.Interfaces;
using PhotoGallery.Core.Domain.Entities;

namespace PhotoGallery.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(UserManager<User> userManager, IJtwTokenGenerator jwtTokenGenerator) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var existingUser = await userManager.FindByEmailAsync(dto.Email);

            if (existingUser != null)
                return BadRequest("User already exists.");

            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email
            };

            var result = await userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await userManager.AddToRoleAsync(user, "User");

            var token = await jwtTokenGenerator.GenerateToken(user);
            var roles = await userManager.GetRolesAsync(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                UserName = user.UserName!,
                Email = user.Email!,
                Roles = roles
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);

            if (user == null) 
                return Unauthorized("Invalid credentials.");

            var passwordValid = await userManager.CheckPasswordAsync(user, dto.Password);

            if (!passwordValid)
                return Unauthorized("Invalid credentials.");

            var token = await jwtTokenGenerator.GenerateToken(user);
            var roles = await userManager.GetRolesAsync(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                UserName = user.UserName!,
                Email = user.Email!,
                Roles = roles
            });
        }
    }
}

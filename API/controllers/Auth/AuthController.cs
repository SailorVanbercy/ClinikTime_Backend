using ClinikTime.service.Auth;
using ClinikTime.service.jwt;
using Infrastructure.user.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinikTime.controllers.Auth;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(IAuthService authService, ITokenService tokenService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginUserDto dto)
    {
        var user = await authService.LoginAsync(dto);
        if (user == null)
        {
            return Unauthorized();
        }

        var token = tokenService.GenerateToken(user);
        
        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(60)
        });
        return Ok(new UserResponseDto(user));
    }
    
    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return Ok("You have been logged out.");
    }
}
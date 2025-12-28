using ClinikTime.service.Auth;
using ClinikTime.service.jwt;
using ClinikTime.service.PasswordReset;
using Infrastructure.user.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinikTime.controllers.Auth;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(IAuthService authService, ITokenService tokenService, PasswordResetService passwordResetService) : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new{
            role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value,
            userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            }
            );
    }
    
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
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Path = "/",
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
    
    //DEMANDE DE RESET
    [HttpPost("password-reset/request")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetDto dto)
    {
        await passwordResetService.RequestResetAsync(dto.Email);

        return Ok();
    }
    
    //CONFIRMATION DU RESET
    [HttpPost("password-reset/confirm")]
    public async Task<IActionResult> ConfirmPasswordReset([FromBody] ConfirmPasswordResetDto dto)
    {
        await passwordResetService.ConfirmResetAsync(dto.Token, dto.NewPassword);
        return Ok();
    }
}
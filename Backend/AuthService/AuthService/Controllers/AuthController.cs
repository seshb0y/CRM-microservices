using ControllerFirst.DTO.Requests;
using ControllerFirst.DTO.Responses;
using CRMSolution.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRMSolution.Controllers;

[ApiController]
[Route("api/v1/auth/")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;

    public AuthController(IAuthService authService, ITokenService tokenService)
    {
        _tokenService = tokenService;
        _authService = authService;
    }

    [HttpPost("login")]
    public async  Task<IActionResult> Login([FromBody] HttpLoginRequest request)
    {
        // var response = await _authService.LoginAsync(request, HttpContext);
        //
        // Response.Cookies.Append("accessToken", response.accessToken);
        // Response.Cookies.Append("refreshToken", response.refreshToken);
        
        
        return Ok("new HttpResult<HttpLoginResponse>(true, response, \"Successfully logged in\")");
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        // var response = await _authService.RefreshTokenAsync(HttpContext);
        return Ok("new HttpResult<RefreshTokenResponse>(true, response, \"Successfully refreshed token\")");
    }

    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // await _authService.LogoutAsync(HttpContext);
        return Ok("new { message = \"Logged out successfully\" }");
    }

}
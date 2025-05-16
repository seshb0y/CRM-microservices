using ApiGateway.DTO.Requests;
using ApiGateway.DTO.Responses;
using ApiGateway.Hubs;
using CRMSolution.Grpc.Users;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ApiGateway.Controllers;


[ApiController]
[Route("api/v1/account/")]
public class AccountController : ControllerBase
{
    private readonly UserService.UserServiceClient _accountService;
    private readonly IHubContext<NotificationHub> _hubContext;

    public AccountController(UserService.UserServiceClient accountService, IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
        _accountService = accountService;
    }

    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var grpcRequest = new RegisterRequest
        {
            Email = request.Email,
            Password = request.Password,
            ConfirmPassword = request.ConfirmPassword,
            Username = request.Username
        };

        var response = await _accountService.RegisterAsync(grpcRequest);

        await _hubContext.Clients.All.SendAsync("NewUserRegistered", new
        {
            response.Id,
            response.Email,
            response.Username,
            response.CreatedAt,
            response.Role,
        });

        return Ok(response);
    }


    // [Authorize(Policy = "AdminPolicy")]
    [HttpGet("email/verify")]
    public async Task<IActionResult> VerifyEmailAsync([FromQuery] string token)
    {
        string? accessToken = Request.Cookies["accessToken"];
        var metadata = new Metadata();
        metadata.Add("token", token);
        var grpcResponse = await _accountService.VerifyEmailAsync(new VerifyEmailRequest(), metadata);
        
        return Ok(new Result<string>(true, "Email confirmed", "Email confirmed"));
    }
    // [Authorize(Policy = "AdminPolicy")]
    [HttpPost("email/confirm")]
    public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmRequest request)
    {
        var grpcRequest = new ConfirmRequest
        {
            Username = request.Username,
        };
        var grpcResponse = await _accountService.ConfirmEmailAsync(grpcRequest);
        
        return Ok(new Result<string>(true, grpcResponse.Username, "Email sent"));
    }

    [HttpPost("password/reset")]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest request)
    {
        var grpcRequest = new ResetPasswordRequest
        {
            Username = request.Username,
        };
        var grpcResponse = await _accountService.ResetPasswordAsync(grpcRequest);
        
        return Ok(new Result<string>(true, request.Username, "Reset password mail sent"));
    }

    [HttpPost("password/change")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] HttpChangePasswordRequest request)
    {
        var metadata = new Metadata();
        metadata.Add("token", request.token);
        var grpcRequest = new ChangePasswordRequest
        {
            NewPassword = request.newPassword,
        };
        var  grpcResponse = await _accountService.ChangePasswordAsync(grpcRequest,  metadata);
        
        return Ok(new Result<string>(true, "Password changed", "Password changed"));
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMeAsync()
    {
        string? accessToken = Request.Cookies["accessToken"];
        var metadata = new Metadata { { "access-token", accessToken } };

        var callOptions = new CallOptions(metadata);

        var response = await _accountService.GetCurrentUserAsync(new DefaultRequest(), callOptions);
            
        return Ok(response);
    }
}
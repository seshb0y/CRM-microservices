using AutoMapper;
using BCrypt.Net;
using CRMSolution.Data.Models;
using CRMSolution.Data.Repository.Interface;
using CRMSolution.DTO.Requests;
using CRMSolution.Services.Interfaces;
using System;
using System.Threading.Tasks;
using ControllerFirst.DTO.Requests;
using ControllerFirst.DTO.Responses;
using CRMSolution.Data.Repository;
using CRMSolution.Data.Repository.UserRep;
using CRMSolution.Grpc.Users;
using Grpc.Core;
using RefreshTokenResponse = CRMSolution.Grpc.Users.RefreshTokenResponse;


namespace CRMSolution.Services.Classes;

public class AuthService : IAuthService
{
    private readonly IUserRep _userRep;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRep userRep, IMapper mapper, ITokenService tokenService, ILogger<AuthService> logger)
    {
        _userRep = userRep;
        _mapper = mapper;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        _logger.LogInformation("Вход в аккаунт: {@Request}", request);
        
        var user = await _userRep.FindByNameAsync(request.Username);
        
        user.RefreshToken = Guid.NewGuid();
        user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(7);
        await _userRep.SaveChangesAsync();
        
        var accessToken = await _tokenService.CreateTokenAsync(user.Username);
        var refreshToken = user.RefreshToken.ToString();
        
        // context.Response.Cookies.Append("accessToken", accessToken, new CookieOptions
        // {
        //     HttpOnly = true,
        //     Secure = true,
        //     SameSite = SameSiteMode.Strict,
        //     Expires = DateTime.UtcNow.AddMinutes(15)
        // });
        //
        // context.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        // {
        //     HttpOnly = true,
        //     Secure = true,
        //     SameSite = SameSiteMode.Strict,
        //     Expires = DateTime.UtcNow.AddDays(7)
        // });

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

    }

    public async Task<RefreshTokenResponse> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        _logger.LogInformation("Обновление токена через gRPC");

        if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Tokens are missing"));

        var username = await _tokenService.GetNameFromToken(accessToken);
        var user = await _userRep.FindByNameAsync(username);

        if (user == null || user.RefreshToken.ToString() != refreshToken || user.RefreshTokenExpiration < DateTime.UtcNow)
            throw new RpcException(new Status(StatusCode.PermissionDenied, "Invalid refresh token"));

        user.RefreshToken = Guid.NewGuid();
        user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(7);
        await _userRep.SaveChangesAsync();

        var newAccessToken = await _tokenService.CreateTokenAsync(user.Username);
        var newRefreshToken = user.RefreshToken.ToString();

        return new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    
    // public Task LogoutAsync(HttpContext context)
    // {
    //     context.Response.Cookies.Delete("accessToken");
    //     context.Response.Cookies.Delete("refreshToken");
    //     return Task.CompletedTask;
    // }
}
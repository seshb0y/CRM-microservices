using ControllerFirst.DTO.Requests;
using ControllerFirst.DTO.Responses;
using CRMSolution.Grpc.Users;
using RefreshTokenResponse = CRMSolution.Grpc.Users.RefreshTokenResponse;

namespace CRMSolution.Services.Interfaces;

public interface IAuthService
{
    public Task<LoginResponse> LoginAsync(LoginRequest request);
    
    public Task<RefreshTokenResponse> RefreshTokenAsync(string accessToken, string refreshToken);

    // public Task LogoutAsync(HttpContext context);
}
using ControllerFirst.DTO.Requests;
using ControllerFirst.DTO.Responses;
using CRMSolution.Data.Models;
using CRMSolution.Grpc.Users;

namespace CRMSolution.Services.Interfaces;

public interface IAccountService
{
    public Task SendEmailAsync(string to, string subject, string html);
    public Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    public Task<ConfirmResponse> ConfirmEmailAsync(ConfirmRequest request);
    public Task VerifyEmailAsync(string token);
    public Task ResetPasswordAsync(ResetPasswordRequest request);
    
    public Task ChangePasswordAsync(ChangePasswordRequest request, string token);
    public Task<CurrentUserResponse> GetCurrentUserAsync(string token);
}
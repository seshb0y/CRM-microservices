namespace CRMSolution.Services.Interfaces;

public interface ITokenService
{
    public Task<string> GetNameFromToken(string token);
    public Task<string> GetNameFromCookies(string token);
    public Task<string> CreateTokenAsync(string username);
    public Task<string> CreateEmailTokenAsync(string username);
    
    public Task<bool> ValidateEmailTokenAsync(string token);
    public Task<string> GetNameFromToken(string token, string key);
    
    public Task<string> CreateResetPasswordTokenAsync(string username);
    public Task<bool> ValidateChangePasswordTokenAsync(string token);
}
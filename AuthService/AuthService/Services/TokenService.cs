using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CRMSolution.Data.Models;
using CRMSolution.Data.Repository.UserRep;
using CRMSolution.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CRMSolution.Services.Classes;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly IUserRep _userRepository;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IConfiguration config, IUserRep userRepository, ILogger<TokenService> logger)
    {
        _config = config;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<string> GetNameFromToken(string token)
    {
        try
        {
            _logger.LogInformation("Берем информацию из токена: {@Token}", token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (securityToken == null)
                throw new SecurityTokenException("Invalid token");

            var username = securityToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
                throw new SecurityTokenException("Username not found in token");

            return username;
        }
        catch (Exception ex)
        {
            _logger.LogError("Ошибка парсинга токена: {Error}", ex.Message);
            throw;
        }
    }
    
    public async Task<string> GetNameFromCookies(string token)
    {
        var accessToken = token;
        if (string.IsNullOrEmpty(accessToken))
            throw new SecurityTokenException("Access token is missing");

        return await GetNameFromToken(accessToken);
    }
    public async Task<string> CreateTokenAsync(string username)
    {
        _logger.LogInformation("Создаем новый токен: {@Username}", username);
        var user = await _userRepository.FindByNameAsync(username);

        if (user == null)
            throw new SecurityTokenException("User not found");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var securityToken = new JwtSecurityToken(
            issuer: _config.GetSection("JWT:Issuer").Get<string[]>()[0],
            audience: _config.GetSection("JWT:Audience").Get<string[]>()[0],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
    
    public async Task<string> CreateEmailTokenAsync(string username)
    {
        _logger.LogInformation("Создаем токен для подтверждения мыла: {@Username}", username);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JWT:EmailKey").Value));

        var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        var securityToken = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            issuer: _config.GetSection("JWT:Issuer").Get<string[]>()[0],
            audience: _config.GetSection("JWT:Audience").Get<string[]>()[0],
            signingCredentials: signingCred);

        string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return tokenString;
    }

    public async Task<string> GetNameFromToken(string token, string key)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (securityToken == null)
                throw new SecurityTokenException("Invalid token");

            var username = securityToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
                throw new SecurityTokenException("Username not found in token");

            return username;
        }
        catch (Exception ex)
        {
            _logger.LogError("Ошибка парсинга токена: {Error}", ex.Message);
            throw;
        }
    }

    public async Task<bool> ValidateEmailTokenAsync(string token)
    {
        _logger.LogInformation("Проверка токена для подтверждения мыла: {@Token}", token);
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JWT:EmailKey").Value));

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _config.GetSection("JWT:Issuer").Get<string[]>()[0],
            ValidAudience = _config.GetSection("JWT:Audience").Get<string[]>()[0],
            IssuerSigningKey = securityKey,
            ClockSkew = TimeSpan.Zero
        };


        var principal = await tokenHandler.ValidateTokenAsync(token, validationParameters);
        return principal.IsValid;
    }

    public async Task<string> CreateResetPasswordTokenAsync(string username)
    {
        _logger.LogInformation("Создаем токен для сброса пароля: {@Username}", username);
    
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:EmailKey"]));
        var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        var securityToken = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            issuer: _config.GetSection("JWT:Issuer").Get<string[]>()[0],
            audience: _config.GetSection("JWT:Audience").Get<string[]>()[0],
            signingCredentials: signingCred
        );

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    
    public async Task<bool> ValidateChangePasswordTokenAsync(string token)
    {
        _logger.LogInformation("Проверка токена для сброса пароля: {@Token}", token);
    
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:EmailKey"]));

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _config.GetSection("JWT:Issuer").Get<string[]>()[0],
            ValidAudience = _config.GetSection("JWT:Audience").Get<string[]>()[0],
            IssuerSigningKey = securityKey,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return true;
        }
        catch (SecurityTokenException)
        {
            return false;
        }
    }

}
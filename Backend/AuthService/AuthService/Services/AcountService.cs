using System.Text;
using AutoMapper;
using CRMSolution.Data.Models;
using CRMSolution.Services.Interfaces;
using ControllerFirst.DTO.Requests;
using ControllerFirst.DTO.Responses;
using CRMSolution.Data.Repository.UserRep;
using CRMSolution.Grpc.Users;
using CRMSolution.Hubs;
using Google.Protobuf.WellKnownTypes;
using MimeKit;
using MailKit.Security;
using Microsoft.AspNetCore.SignalR;
using RegisterRequest = CRMSolution.Grpc.Users.RegisterRequest;
using UserRole = CRMSolution.Data.Models.UserRole;

namespace CRMSolution.Services.Classes;

public class AccountService : IAccountService
{
    private readonly IMapper _mapper;
    private readonly IUserRep _userRep;
    private readonly IConfiguration _config;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AccountService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHubContext<NotificationHub> _notificationHub;

    public AccountService(IMapper mapper, IUserRep userRep, IConfiguration config, ITokenService tokenService,
        ILogger<AccountService> logger,  IHttpContextAccessor httpContextAccessor, IHubContext<NotificationHub> notificationHub)
    {
        _mapper = mapper;
        _userRep = userRep;
        _config = config;
        _tokenService = tokenService;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _notificationHub = notificationHub;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        _logger.LogInformation("Регистрация юзера: {@Request}", request);
        if (await _userRep.FindByNameAsync(request.Username) != null || 
            await _userRep.FindByEmailAsync(request.Email) != null)
        {
            throw new Exception("User with this email or username already exists");
        }


        var user = _mapper.Map<User>(request);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        user.Role = UserRole.Manager; 

        await _userRep.AddAsync(user);
        await _userRep.SaveChangesAsync();
        return new RegisterResponse
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            CreatedAt = Timestamp.FromDateTime(user.CreatedAt.ToUniversalTime()) ,
            Role = (Grpc.Users.UserRole)user.Role
        };

    }

    public async Task<ConfirmResponse> ConfirmEmailAsync(ConfirmRequest request)
    {
        _logger.LogInformation("Отправка письма для подтверждения мыла: {@Request}", request);
        var user = await _userRep.FindByNameAsync(request.Username);
        if (user == null)
            throw new Exception("User not found");

        string token = await _tokenService.CreateEmailTokenAsync(request.Username);
        string link = $"http://localhost:5173/verify-email?token={token}";

        string emailBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <h2 style='color: #4F46E5;'>Email Confirmation</h2>
                <p>Hello, {request.Username}!</p>
                <p>Please confirm your email address by clicking the button below:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <a href='{link}' style='background-color: #4F46E5; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block;'>
                        Confirm Email
                    </a>
                </div>
                <p style='color: #666; font-size: 14px;'>If the button doesn't work, you can copy and paste this link into your browser:</p>
                <p style='color: #666; font-size: 14px;'>{link}</p>
                <p style='color: #666; font-size: 12px; margin-top: 30px;'>This link will expire in 24 hours.</p>
            </div>";
        await SendEmailAsync(user.Email, "Email Confirmation", emailBody);
        return new ConfirmResponse
        {
            Username = user.Username,
        };
    }

    public async Task VerifyEmailAsync(string token)
    {
        _logger.LogInformation("Подтверждение мыла по токены: {@Token}", token);
        string username = await _tokenService.GetNameFromToken(token, _config["JWT:EmailKey"]);
        if (string.IsNullOrEmpty(username))
            throw new Exception("Invalid token");

        bool isValid = await _tokenService.ValidateEmailTokenAsync(token);
        if (!isValid)
            throw new Exception("Token is invalid or expired");

        var user = await _userRep.FindByNameAsync(username);
        if (user == null)
            throw new Exception("User not found");

        user.IsEmailConfirmed = true;
        await _userRep.SaveChangesAsync();
    }
    

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        _logger.LogInformation("Отправка письма для сброса пароля: {@Request}", request);
        var user = await _userRep.FindByNameAsync(request.Username);
        if (user == null)
            throw new Exception("User not found");

        string token = await _tokenService.CreateResetPasswordTokenAsync(request.Username);
        string link = $"http://localhost:5173/change-password?token={token}";

        string emailBody = $"<p>Привет, {request.Username}! Чтобы сбросить пароль, перейдите <a href='{link}'>сюда</a>.</p>";
        await SendEmailAsync(user.Email, "Сброс пароля", emailBody);
    }

    public async Task ChangePasswordAsync(ChangePasswordRequest request, string token)
    {
        _logger.LogInformation("Изменение пароля: {@Request}", request);
        bool isValid = await _tokenService.ValidateChangePasswordTokenAsync(token);
        if (!isValid)
            throw new Exception("Invalid or expired token");

        var username = await _tokenService.GetNameFromToken(token);
        var user = await _userRep.FindByNameAsync(username);
        if (user == null)
            throw new Exception("User not found");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _userRep.SaveChangesAsync();
    }
    
    public async Task<CurrentUserResponse> GetCurrentUserAsync(string token)
    {

        string username = await _tokenService.GetNameFromToken(token);
        if (string.IsNullOrEmpty(username))
        {
            throw new Exception("Invalid token");
        }

        var user = await _userRep.FindByNameAsync(username);
        
        
        return _mapper.Map<CurrentUserResponse>(user);
    }
    
    public async Task SendEmailAsync(string to, string subject, string html)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_config["Smtp:SenderEmail"]));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart("html") { Text = html };

        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        var port = int.Parse(_config["Smtp:Port"]);
        var host = _config["Smtp:Host"];
        var username = _config["Smtp:Username"];
        var password = _config["Smtp:Password"];

        var socketOption = _config["Smtp:SecureSocketOption"]?.ToLower() switch
        {
            "none" => SecureSocketOptions.None,
            "ssl" => SecureSocketOptions.SslOnConnect,
            "starttls" => SecureSocketOptions.StartTls,
            "starttlswhenavailable" => SecureSocketOptions.StartTlsWhenAvailable,
            _ => SecureSocketOptions.StartTls
        };
        
        await smtp.ConnectAsync(host, port, socketOption);
        smtp.AuthenticationMechanisms.Remove("XOAUTH2"); 
        await smtp.AuthenticateAsync(username, password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }

}

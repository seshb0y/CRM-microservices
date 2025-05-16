using AutoMapper;
using Grpc.Core;
using CRMSolution.Data.Repository.UserRep;
using CRMSolution.Data.Validators.Auth;
using CRMSolution.Data.Validators.User;
using CRMSolution.Grpc.Users;
using CRMSolution.Services.Interfaces;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using UserRole = CRMSolution.Grpc.Users.UserRole;

public class UserGrpcService : UserService.UserServiceBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserGrpcService> _logger;
    private readonly IAccountService _accountService;
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly IUserRep _userRep;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly IValidator<ChangePasswordRequest> _changePasswordValidator;
    private readonly IValidator<ChangeUserDataRequest> _changeUserDataValidator;

    public UserGrpcService(IUserService userService, ILogger<UserGrpcService> logger, 
        IAccountService accountService, IAuthService authService, ITokenService tokenService, IUserRep userRep,
        IValidator<RegisterRequest> registerValidator, IValidator<LoginRequest> loginValidator,
        IValidator<ChangePasswordRequest> changePasswordValidator, IValidator<ChangeUserDataRequest> changeUserDataValidator)
    {
        _userService =  userService;
        _logger = logger;
        _accountService = accountService;
        _authService = authService;
        _tokenService = tokenService;
        _userRep = userRep;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _changePasswordValidator = changePasswordValidator;
        _changeUserDataValidator = changeUserDataValidator;
    }

    public override async Task<GetUserResponse> GetUserById(GetUserByIdRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC запрос на поиск пользователя по ID: {UserId}", request.Id);

        var user = await _userService.GetByIdAsync(request.Id);
        
        return new GetUserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = (UserRole)user.Role
        };
    }

    public override async Task<GetUserResponse> GetUserByUsername(GetUserByEmailRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC FindUser: {username}", request.Email);
        return await _userService.GetUserByUsername(request);
    }
    public override async Task<FindUserResponse> FindUser(GetUserByEmailRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC FindUser: {Email}", request.Email);
        var userData = await _userService.FindUser(request);
        return userData;
    }

    public override async Task<GetUsersByIdsResponse> GetUsersByIds(GetUsersByIdsRequest request,
        ServerCallContext context)
    {
        return await _userService.GetUsersByIds(request);
    }
    
    public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        var result = await _registerValidator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = string.Join(" | ", result.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
        }
        
        return await _accountService.RegisterAsync(request);
    }

    public override async Task<ConfirmResponse> ConfirmEmail(ConfirmRequest request, ServerCallContext context)
    {
        return await _accountService.ConfirmEmailAsync(request);
    }

    public override async Task<DefaultResponse> VerifyEmail(VerifyEmailRequest request, ServerCallContext context)
    {
        var token = context.RequestHeaders.FirstOrDefault(h => h.Key == "token").Value;
        await _accountService.VerifyEmailAsync(token);
        return new DefaultResponse
        {
            Message = "verify email success",
            Success = true
        };
    }

    public override async Task<DefaultResponse> ResetPassword(ResetPasswordRequest request, ServerCallContext context)
    {
        await _accountService.ResetPasswordAsync(request);
        return new DefaultResponse
        {
            Message = "reset password success",
            Success = true
        };
    }

    public override async Task<DefaultResponse> ChangePassword(ChangePasswordRequest request, ServerCallContext context)
    {
        var result = await _changePasswordValidator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = string.Join(" | ", result.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
        }
        
        var token = context.RequestHeaders.FirstOrDefault(h => h.Key == "token").Value;
        await _accountService.ChangePasswordAsync(request, token);
        return new DefaultResponse
        {
            Message = "password change success",
            Success = true
        };
    }

    public override async Task<CurrentUserResponse> GetCurrentUser(DefaultRequest request, ServerCallContext context)
    {
        var token = context.RequestHeaders.FirstOrDefault(h => h.Key == "access-token").Value;
        return await _accountService.GetCurrentUserAsync(token);
    }

    public override async Task<DefaultResponse> SendEmail(SendEmailRequest request, ServerCallContext context)
    {
        await _accountService.SendEmailAsync(request.To, request.Html, request.Subject);
        return new DefaultResponse
        {
            Message = "email send success",
            Success = true
        };
    }

    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var result = await _loginValidator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = string.Join(" | ", result.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
        }
        return await _authService.LoginAsync(request);
    }

    public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request,
        ServerCallContext context)
    {
        var accessToken = context.RequestHeaders.FirstOrDefault(h => h.Key == "accesstoken")?.Value;
        var refreshToken = context.RequestHeaders.FirstOrDefault(h => h.Key == "refreshtoken")?.Value;
        return await _authService.RefreshTokenAsync(accessToken, refreshToken);
    }

    public override async Task<GetNameFromTokenResponse> GetNameFromToken(GetNameFromTokenRequest request,
        ServerCallContext context)
    {
        return new GetNameFromTokenResponse
        {
            Username = await _tokenService.GetNameFromToken(request.Token)
        };
    }

    public override async Task<CreateTokenResponse> CreateToken(CreateTokenRequest request, ServerCallContext context)
    {
        return new CreateTokenResponse
        {
            Token = await _tokenService.CreateTokenAsync(request.Username)
        };
    }
    public override async Task<CreateTokenResponse> CreateEmailToken(CreateTokenRequest request, ServerCallContext context)
    {
        return new CreateTokenResponse
        {
            Token = await _tokenService.CreateTokenAsync(request.Username)
        };
    }

    public override async Task<ValidateTokenResponse> ValidateEmailToken(ValidateTokenRequest request,
        ServerCallContext context)
    {
        return new ValidateTokenResponse
        {
            IsValidate = await _tokenService.ValidateEmailTokenAsync(request.Token)
        };
    }

    public override async Task<CreateTokenResponse> CreateResetPasswordToken(CreateTokenRequest request,
        ServerCallContext context)
    {
        return new CreateTokenResponse
        {
            Token = await _tokenService.CreateTokenAsync(request.Username)
        };
    }
    public override async Task<ValidateTokenResponse> ValidatePasswordTokenAsync(ValidateTokenRequest request,
        ServerCallContext context)
    {
        return new ValidateTokenResponse
        {
            IsValidate = await _tokenService.ValidateChangePasswordTokenAsync(request.Token)
        };
    }

    public override async Task<ChangeUserDataResponse> ChangeUserData(ChangeUserDataRequest request,
        ServerCallContext context)
    { 
        var result = await _changeUserDataValidator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = string.Join(" | ", result.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
        }
        var changedUser = await _userService.ChangeUserData(request);
        return new ChangeUserDataResponse
        {
            Username = changedUser.Username,
            Email = changedUser.Email,
            Role = changedUser.Role,
            Id = changedUser.Id,
            CreatedAt = Timestamp.FromDateTime(changedUser.CreatedAt.ToDateTime().ToUniversalTime())
        };
    }

    public override async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
    {
        return await _userService.DeleteUser(request);
    }

    public override async Task<GetAllUsersResponse> GetAllUsers(GetAllUsersRequest request, ServerCallContext context)
    {
        return await _userService.GetAllUsers(request.Sort);
    }
}
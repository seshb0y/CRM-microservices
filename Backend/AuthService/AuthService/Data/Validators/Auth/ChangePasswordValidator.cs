using ControllerFirst.DTO.Requests;
using ControllerFirst.Shared;
using CRMSolution.Grpc.Users;
using FluentValidation;

namespace CRMSolution.Data.Validators.Auth;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordValidator()
    {
        RuleFor(r => r.NewPassword)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .Matches(RegexPattern.Password)
            .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, and one number");
        
    }
}
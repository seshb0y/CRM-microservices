using ControllerFirst.Shared;
using CRMSolution.Data.Repository.UserRep;
using CRMSolution.DTO.Requests;
using CRMSolution.Grpc.Users;
using FluentValidation;

namespace CRMSolution.Data.Validators.User;

public class ChangeUserDataValidator : AbstractValidator<ChangeUserDataRequest>
{
    IUserRep _userRep;
    
    public ChangeUserDataValidator(IUserRep userRep)
    {
        _userRep = userRep;
        
        RuleFor(x => x.Username)
            .MinimumLength(2).WithMessage("Name must be at least 2 characters");

        RuleFor(x => x.NewEmail)
            .EmailAddress().WithMessage("Invalid email address");

        RuleFor(x => x.OldEmail)
            .MustAsync(IsClientExist)
            .WithMessage("The client email does not exist.");
        
    }

    private async Task<bool> IsClientExist(string email, CancellationToken cancellationToken)
    {
        var client = await _userRep.FindByEmailAsync(email);
        return client != null;
    }
}
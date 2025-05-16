using ClientService.Data.Repository.SpecialRepClass.ClientRep;
using CRMSolution.DTO.Requests.Client;
using CRMSolution.Grpc.Client;
using FluentValidation;

namespace CRMSolution.Data.Validators;

public class ChangeDataClientValidator : AbstractValidator<ChangeDataClientRequest>
{
    IClientRep _clientRepository;
    
    public ChangeDataClientValidator(IClientRep clientRepository)
    {
        _clientRepository = clientRepository;
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("Phone is required")
            .Matches(@"^\+?[0-9]{7,15}$").WithMessage("Invalid phone number");

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Address is required")
            .MaximumLength(255).WithMessage("Address is too long");

        RuleFor(x => x.OldEmail)
            .NotEmpty()
            .WithMessage("Email is required")
            .MustAsync(IsClientExist)
            .WithMessage("The client email does not exist.");
    }

    private async Task<bool> IsClientExist(string email, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetClientByEmail(email);
        return client != null;
    }
}
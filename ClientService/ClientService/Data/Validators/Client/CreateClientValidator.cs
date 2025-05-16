using CRMSolution.DTO.Requests.Client;
using CRMSolution.Grpc.Client;
using FluentValidation;

namespace CRMSolution.Data.Validators;

public class CreateClientValidator : AbstractValidator<CreateClientRequest>
{
    public CreateClientValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(2)
            .WithMessage("Name must be at least 2 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email address");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("Phone is required")
            .Matches(@"^\+?[0-9]{7,15}$")
            .WithMessage("Invalid phone number");

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Address is required")
            .MaximumLength(255)
            .WithMessage("Address is too long");
    }
}
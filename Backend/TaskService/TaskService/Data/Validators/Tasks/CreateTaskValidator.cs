
using CRMSolution.Grpc.Tasks;
using FluentValidation;

namespace CRMSolution.Data.Validators.Tasks;

public class CreateTaskValidator : AbstractValidator<CreateTaskRequest>
{

    public CreateTaskValidator()
    {

        RuleFor(r => r.Title)
            .NotNull()
            .WithMessage("Title is required.")
            .NotEmpty()
            .WithMessage("Title is required.");
        
        RuleFor(r => r.Description)
            .NotNull()
            .WithMessage("Description is required.")
            .NotEmpty()
            .WithMessage("Description is required.");
        
        RuleFor(r => r.DueDate)
            .NotNull()
            .WithMessage("End date is required.")
            .NotEmpty()
            .WithMessage("End date is required.");
    }
}
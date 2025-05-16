using ClientService.Data.Repository.SpecialRepClass.ClientRep;
using CRMSolution.Grpc.Client;
using FluentValidation;

namespace CRMSolution.Data.Validators;

public class DeleteClientValidator : AbstractValidator<DeleteClientRequest>
{
    IClientRep _clientRepository;
    
    public DeleteClientValidator(IClientRep clientRepository)
    {
        _clientRepository = clientRepository;
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .MustAsync(IsClientExist)
            .WithMessage("The client ID does not exist.");
    }
    
    private async Task<bool> IsClientExist(string email, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetClientByEmail(email);
        return client != null;
    }
    
}
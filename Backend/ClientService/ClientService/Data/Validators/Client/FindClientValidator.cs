using ClientService.Data.Repository.SpecialRepClass.ClientRep;
using CRMSolution.DTO.Requests.Client;
using CRMSolution.Grpc.Client;
using FluentValidation;

namespace CRMSolution.Data.Validators;

public class FindClientValidator : AbstractValidator<GetClientByEmailRequest>
{
    IClientRep _clientRepository;
    
    public FindClientValidator(IClientRep clientRepository)
    {
        _clientRepository = clientRepository;
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Id is required")
            .MustAsync(IsClientExist)
            .WithMessage("The client id does not exist.");
    }
    private async Task<bool> IsClientExist(string email, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetClientByEmail(email);
        return client != null;
    }
}
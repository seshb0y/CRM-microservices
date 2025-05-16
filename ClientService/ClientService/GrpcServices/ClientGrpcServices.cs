using AutoMapper;
using ClientService.Data.Repository.SpecialRepClass.ClientRep;
using ClientService.Services.Interfaces;
using CRMSolution.Data.Validators;
using CRMSolution.Grpc.Client;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace ClientService.GrpcServices;

public class ClientGrpcService : CRMSolution.Grpc.Client.ClientGrpcService.ClientGrpcServiceBase
{
    private readonly IClientService _clientService;
    private readonly ILogger<ClientGrpcService> _logger;
    private readonly IClientRep _clientRep;
    private readonly IValidator<ChangeDataClientRequest> _changeDataClientValidator;
    private readonly IValidator<CreateClientRequest> _createClientValidator;
    private readonly IValidator<DeleteClientRequest> _deleteValidator;
    private readonly IValidator<GetClientByEmailRequest> _getClientByEmailValidator;

    public ClientGrpcService(IClientService clientService,  ILogger<ClientGrpcService> logger,  IClientRep clientRep,
        IValidator<ChangeDataClientRequest> changeDataClientValidator, IValidator<CreateClientRequest> createClientValidator,
        IValidator<DeleteClientRequest> deleteValidator, IValidator<GetClientByEmailRequest> getClientByEmailValidator)
    {
        _clientService = clientService;
        _logger = logger;
        _clientRep = clientRep;
        _changeDataClientValidator = changeDataClientValidator;
        _createClientValidator = createClientValidator;
        _deleteValidator = deleteValidator;
        _getClientByEmailValidator = getClientByEmailValidator;
    }

    public override async Task<GetClientResponse> GetClientById(GetClientByIdRequest request, ServerCallContext context)
    {
        var client = await _clientService.GetByIdAsync(request);
        return new GetClientResponse
        {
            Id = client.Id,
            Name = client.Name,
            Email = client.Email,
            Phone = client.Phone,
            Address = client.Address,
            CreatedAt = Timestamp.FromDateTime(client.CreatedAt.ToUniversalTime())
        };
    }
    public override async Task<GetClientResponse> GetClientByEmail(GetClientByEmailRequest request, ServerCallContext context)
    {
        var result = await _getClientByEmailValidator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = string.Join(" | ", result.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
        }
        _logger.LogInformation("gRPC запрос на поиск клиента по Email: {Email}", request.Email);
        var client = await _clientService.GetByEmailAsync(request);

        return new GetClientResponse
        {
            Id = client.Id,
            Name = client.Name,
            Email = client.Email,
            Phone = client.Phone,
            Address = client.Address,
            CreatedAt = Timestamp.FromDateTime(client.CreatedAt.ToUniversalTime())
        };

    }

    public override async Task<GetClientsByIdsResponse> GetClientsByIds(GetClientsByIdsRequest request,
        ServerCallContext context)
    {
        return await _clientService.GetClientsByIds(request);
    }

    public override async Task<CreateClientResponse> CreateClient(CreateClientRequest request,
        ServerCallContext context)
    {
        var result = await _createClientValidator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = string.Join(" | ", result.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
        }
        return await _clientService.CreateClient(request);
    }

    public override async Task<ChangeDataClientResponse> ChangeDataClient(ChangeDataClientRequest request,
        ServerCallContext context)
    {
        var result = await _changeDataClientValidator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = string.Join(" | ", result.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
        }
        
        return await _clientService.ChangeDataClient(request);
    }

    public override async Task<DeleteClientResponse> DeleteClient(DeleteClientRequest request,
        ServerCallContext context)
    {
        var validator = new DeleteClientValidator(_clientRep);
        var result = await _deleteValidator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = string.Join(" | ", result.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
        }
        return await _clientService.DeleteClient(request);
    }

    public override async Task<GetAllClientsResponse> GetAllClients(GetAllClientsRequest request,
        ServerCallContext context)
    {
        return await _clientService.GetAllClients(request);
    }

    public override async Task<GetClientsWithOrdersAndTasksResponse> GetClientsWithOrdersAndTasks(
        GetClientWithOrdersAndTasksRequest request,
        ServerCallContext context)
    {
        var token = context.RequestHeaders.FirstOrDefault(h => h.Key == "authorization").Value;
        return await _clientService.GetClientsWithOrdersAndTasksAsync(token);
    }

    public override async Task<GetDashboardDataResponse> GetDashboardData(GetDashboardDataRequest request, ServerCallContext context)
    {
        return await _clientService.GetDashboardData(request);
    }
}
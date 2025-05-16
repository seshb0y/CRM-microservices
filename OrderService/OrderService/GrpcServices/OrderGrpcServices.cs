using AutoMapper;
using CRMSolution.Grpc.Client;
using CRMSolution.Grpc.Orders;
using CRMSolution.Grpc.Tasks;
using CRMSolution.Grpc.Users;
using FluentValidation;
using Grpc.Core;
using OrderService.Data.Repository.OrderResp;
using OrderService.Data.Validators.Order;
using OrderService.Services.Interfaces;
using OrderService.Data.Models;
using OrderService.DTO.Requests; // <-- Только этот using правильный!
using OrderService.DTO.Requests.Order;
using OrderStatus = CRMSolution.Grpc.Orders.OrderStatus;

namespace OrderService.GrpcServices;

public class OrderGrpcService : CRMSolution.Grpc.Orders.OrderGrpcService.OrderGrpcServiceBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderGrpcService> _logger;
    private readonly UserService.UserServiceClient _userGrpcClient;
    private readonly ClientGrpcService.ClientGrpcServiceClient _clientGrpcClient;
    private readonly TaskGrpcService.TaskGrpcServiceClient _taskGrpcService;
    private readonly IMapper _mapper;
    private readonly IOrderRep _orderRep;
    private readonly IValidator<ChangeOrderDataRequest>  _changeOrderDataRequestValidator;
    private readonly IValidator<CreateOrderRequest>  _createOrderRequestValidator;
    private readonly IValidator<DeleteOrderRequest>  _deleteOrderRequestValidator;
    private readonly IValidator<GetOrderByIdRequest>  _findOrderRequestValidator;

    public OrderGrpcService(IOrderService orderService,  ILogger<OrderGrpcService> logger, UserService.UserServiceClient userGrpcClient,
        ClientGrpcService.ClientGrpcServiceClient clientGrpcClient,  TaskGrpcService.TaskGrpcServiceClient taskGrpcService,
        IMapper mapper,  IOrderRep orderRep,  IValidator<ChangeOrderDataRequest> changeOrderDataRequestValidator,
        IValidator<CreateOrderRequest> createOrderRequestValidator,  IValidator<DeleteOrderRequest> deleteOrderRequestValidator,
        IValidator<GetOrderByIdRequest> findOrderRequestValidator)
    {
        _orderService = orderService;
        _logger = logger;
        _userGrpcClient = userGrpcClient;
        _clientGrpcClient = clientGrpcClient;
        _taskGrpcService = taskGrpcService;
        _mapper = mapper;
        _orderRep = orderRep;
        _changeOrderDataRequestValidator = changeOrderDataRequestValidator;
        _createOrderRequestValidator = createOrderRequestValidator;
        _deleteOrderRequestValidator = deleteOrderRequestValidator;
        _findOrderRequestValidator = findOrderRequestValidator;
    }

    public override async Task<OrderDto> GetOrderById(GetOrderByIdRequest request, ServerCallContext context)
    {
        var result = await _findOrderRequestValidator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = string.Join(" | ", result.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
        }
        var order = await _orderService.GetByIdAsync(request.OrderId);

        return new OrderDto
        {
            Id = order.Id.ToString(),
            TotalAmount = (double)order.TotalAmount,
            Status = order.Status.ToString(),
            UserId = order.UserId ?? 0,
            ClientId = order.ClientId ?? 0,
        };
    }

    public override async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request,
        ServerCallContext context)
    {
        var validator = new CreateOrderValidator();
        var result = await _createOrderRequestValidator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = string.Join(" | ", result.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
        }
        return await _orderService.CreateOrder(request);
    }

    public override async Task<ChangeOrderDataResponse> ChangeOrderData(ChangeOrderDataRequest request,
        ServerCallContext context)
    {
        var validator = new ChangeOrderDataValidator(_orderRep);
        var result = await _changeOrderDataRequestValidator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = string.Join(" | ", result.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
        }
        return await _orderService.ChangeDataOrder(request);
    }

    public override async Task<DeleteOrderResponse> DeleteOrder(DeleteOrderRequest request, ServerCallContext context)
    {
        var validator = new DeleteOrderValidator(_orderRep);
        var result = await _deleteOrderRequestValidator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = string.Join(" | ", result.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
        }
        await _orderService.DeleteOrder(request);
        
        return new DeleteOrderResponse
        {
            Success = true,
            Message = "Order updated"
        }; 
    }

    public override async Task<GetOrderFullInfoResponse> GetOrderFullInfo(GetOrderFullInfoRequest request,
        ServerCallContext context)
    {
        return await _orderService.GetOrderInfo(request);
    }
    
    public override async Task<GetLowInfoOrdersListResponse> GetLowInfoOrdersList(GetLowInfoOrdersListRequest request, ServerCallContext context)
    {
        return await _orderService.GetLowInfoOrdersAsync(request.Sort);
    }

    public override async Task<GetOrdersByUserIdsResponse> GetOrdersByUserIds(GetOrdersByUserIdsRequest request,
        ServerCallContext context)
    {
        return await _orderService.GetOrdersByUserIds(request);
    }

    public override async Task<ChangeResponsibleResponse> ChangeResponsible(ChangeResponsibleRequest request, ServerCallContext context)
    {
        return await _orderService.ChangeResponsible(request);
    }

    public override async Task<SaveCallRecordResponse> SaveCallRecord(SaveCallRecordRequest request,
        ServerCallContext context)
    {
        await _orderService.SaveCallRecord(request);
        return new SaveCallRecordResponse();
    }
}
using AutoMapper;
using CRMSolution.Grpc.Orders;
using CRMSolution.Grpc.Tasks;
using OrderService.Data.Models;
using OrderService.DTO.Requests;
using OrderService.DTO.Responses;
using ChangeOrderRequestGrpc = CRMSolution.Grpc.Orders.ChangeOrderDataRequest;
using CreateOrderRequestGrpc = CRMSolution.Grpc.Orders.CreateOrderRequest;
using DeleteOrderRequestGrpc = CRMSolution.Grpc.Orders.DeleteOrderRequest;
using FindOrderRequestDto = OrderService.DTO.Requests.FindOrderRequest;
using HttpChangeOrderDataRequestDto = OrderService.DTO.Requests.HttpChangeOrderDataRequest;
using HttpCreateOrderRequestDto = OrderService.DTO.Requests.HttpCreateOrderRequest;
using HttpDeleteOrderRequestDto = OrderService.DTO.Requests.HttpDeleteOrderRequest;

namespace OrderService.Data.Mapping;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        // gRPC: создание
        CreateMap<CreateOrderRequestGrpc, Order>()
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => (decimal)src.TotalAmount))
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.ClientId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CallRecord, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // HTTP: создание
        CreateMap<HttpCreateOrderRequestDto, Order>()
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.totalAmount))
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.ClientId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CallRecord, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // gRPC: изменение
        CreateMap<ChangeOrderRequestGrpc, Order>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (OrderService.Data.Models.OrderStatus)src.Status))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => (decimal)src.TotalAmount))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.ClientId, opt => opt.Ignore())
            .ForMember(dest => dest.CallRecord, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // HTTP: изменение
        CreateMap<HttpChangeOrderDataRequestDto, Order>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.orderId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.status))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.totalAmount))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.ClientId, opt => opt.Ignore())
            .ForMember(dest => dest.CallRecord, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // gRPC: удаление
        CreateMap<DeleteOrderRequestGrpc, Order>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CallRecord, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.ClientId, opt => opt.Ignore());

        // HTTP: удаление
        CreateMap<HttpDeleteOrderRequestDto, Order>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.orderId))
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CallRecord, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.ClientId, opt => opt.Ignore());

        // HTTP: поиск
        CreateMap<FindOrderRequestDto, Order>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.orderId))
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CallRecord, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.ClientId, opt => opt.Ignore());

        // gRPC задача
        CreateMap<GetTaskByIdResponse, TaskDto>();
    }
}

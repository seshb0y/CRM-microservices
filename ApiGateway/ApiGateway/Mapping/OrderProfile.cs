using ApiGateway.DTO.Responses;
using AutoMapper;
using CRMSolution.DTO.Requests;
using CRMSolution.Grpc.Orders;
using TaskDto = CRMSolution.Grpc.Orders.TaskDto;
using UserDto = CRMSolution.Grpc.Orders.UserDto;

namespace CRMSolution.Data.Mapping;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        // CreateMap<CreateOrderRequest, Order>()
        //     .ForMember(dest => dest.TotalAmount, opt =>
        //         opt.MapFrom(src => src.totalAmount));
        //
        // CreateMap<ChangeOrderDataRequest, Order>()
        //     .ForMember(dest => dest.Status, opt => opt
        //         .MapFrom(src => src.status))
        //     .ForMember(dest => dest.TotalAmount, opt =>
        //         opt.MapFrom(src => src.totalAmount))
        //     .ForMember(dest => dest.Id, opt => opt
        //         .MapFrom(src => src.orderId));
        //
        //
        // CreateMap<DeleteOrderRequest, Order>()
        //     .ForMember(dest => dest.Id, opt => opt
        //         .MapFrom(src => src.orderId));
        //
        // CreateMap<FindOrderRequest, Order>()
        //     .ForMember(dest => dest.Id, opt => opt
        //         .MapFrom(src => src.orderId));
        //
        // CreateMap<Order, OrderResponse>()
        //     .ForMember(dest => dest.ClientOrders, opt => opt
        //         .MapFrom(src => src.ClientOrders));
        //
        // CreateMap<ClientOrder, ClientOrderDto>()
        //     .ForMember(dest => dest.ClientId, opt => opt
        //         .MapFrom(src => src.ClientId))
        //     .ForMember(dest => dest.ClientName, opt => opt
        //         .MapFrom(src => src.Client.Name))
        //     .ForMember(dest => dest.ClientEmail, opt => opt
        //         .MapFrom(src => src.Client.Email))
        //     .ForMember(dest => dest.ClientPhone, opt => opt
        //         .MapFrom(src => src.Client.Phone))
        //     .ForMember(dest => dest.CreatedAt, opt => opt
        //         .MapFrom(src => src.Client.CreatedAt.ToString()))
        //     .ForMember(dest => dest.ClientAddress, opt => opt
        //         .MapFrom(src => src.Client.Address));

        CreateMap<LowInfoOrder, OrderDTO>()
            .ForMember(dest => dest.TotalAmount, opt => opt.
                MapFrom(src => (decimal)src.TotalAmount))
            .ForMember(dest => dest.ClientName, opt => opt.
                MapFrom(src => src.ClientName))
            .ForMember(dest => dest.Username, opt => opt.
                MapFrom(src => src.Username))
            .ForMember(dest => dest.Status, opt => opt.
                MapFrom(src => src.Status))
            .ForMember(dest => dest.Id,  opt => opt.
                MapFrom(src => src.Id))
            .ForMember(dest => dest.CreatedAt, opt => opt.
                MapFrom(src => src.CreatedAt.ToDateTime()));
        
        CreateMap<GetOrderFullInfoResponse, OrderDetailsResponse>()
            .ForMember(dest => dest.Id, opt => opt.
                MapFrom(src => src.OrderId))
            .ForMember(dest => dest.TotalAmount, opt => opt.
                MapFrom(src => src.OrderTotalAmount))
            .ForMember(dest => dest.Status, opt => opt.
                MapFrom(src => src.OrderStatus))
            .ForMember(dest => dest.Client, opt => opt.
                MapFrom(src => src.Client))
            .ForMember(dest => dest.Tasks, opt => opt.
                MapFrom(src => src.Tasks))
            .ForMember(dest => dest.CallRecordingUrl, opt => opt.
                MapFrom(src => src.CallRecordingUrl))
            .ForMember(dest => dest.Users, opt => opt.
                MapFrom(src => src.Users));

        CreateMap<ClientDto, ClientResponse>()
            .ForMember(dest => dest.CreatedAt, opt => opt.
                MapFrom(src => src.ClientCreatedAt.ToDateTime()));

        CreateMap<TaskDto, OrderDetailsTaskResponse>()
            .ForMember(dest => dest.DueDate, opt => opt.
                MapFrom(src => src.DueDate.ToDateTime()));

        CreateMap<UserDto, OrderDetailsUserResponse>();

    }
}
using AutoMapper;
using CRMSolution.DTO.Requests.Client;
using System.Linq;
using ClientService.Data.Models;
using CRMSolution.Grpc.Client;

namespace CRMSolution.Data.Mapping
{
    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            CreateMap<CreateClientRequest, Client>()
                .ForMember(dest => dest.Name, opt => opt
                    .MapFrom(src => src.Name))
                .ForMember(dest => dest.Address, opt => opt
                    .MapFrom(src => src.Address))
                .ForMember(dest => dest.Email, opt => opt
                    .MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt
                    .MapFrom(src => src.Phone));
            
            CreateMap<ChangeDataClientRequest, Client>()
                .ForMember(dest => dest.Name, opt => opt
                    .MapFrom(src => src.Name))
                .ForMember(dest => dest.Address, opt => opt
                    .MapFrom(src => src.Address))
                .ForMember(dest => dest.Email, opt => opt
                    .MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt
                    .MapFrom(src => src.Phone));
            
            CreateMap<Client, GetClientResponse>()
                .ForMember(dest => dest.Name, opt => opt
                    .MapFrom(src => src.Name))
                .ForMember(dest => dest.Address, opt => opt
                    .MapFrom(src => src.Address))
                .ForMember(dest => dest.Email, opt => opt
                    .MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt
                    .MapFrom(src => src.Phone));
            //
            // CreateMap<Client, ClientWithOrdersAndTasksResponse>()
            //     .ForMember(dest => dest.Orders, opt => opt
            //         .MapFrom(src => src.ClientOrders.Select(co => co.Order).ToList()));
            //
            //
            // CreateMap<Order, KanbanOrderResponse>()
            //     .ForMember(dest => dest.OrderId, opt => opt
            //         .MapFrom(src => src.Id))
            //     .ForMember(dest => dest.OrderStatus, opt => opt
            //         .MapFrom(src => src.Status.ToString()))
            //     .ForMember(dest => dest.Tasks, opt => opt
            //         .MapFrom(src => src.Tasks));
            //
            //
            //
            // CreateMap<Tasks, KanbanTaskResponse>()
            //     .ForMember(dest => dest.TaskId, opt => opt
            //         .MapFrom(src => src.Id))
            //     .ForMember(dest => dest.Status, opt => opt
            //         .MapFrom(src => src.Status.ToString()));
            //
            //
            // CreateMap<Client, ClientWithOrdersAndTasksResponse>()
            //     .ForMember(dest => dest.CreatedAt, opt => opt
            //         .MapFrom(src => src.CreatedAt))
            //     .ForMember(dest => dest.Orders, opt => opt
            //         .MapFrom(src => src.ClientOrders.Select(co => co.Order).ToList()));
            //
            // CreateMap<Order, KanbanOrderResponse>()
            //     .ForMember(dest => dest.OrderId, opt => opt
            //         .MapFrom(src => src.Id))
            //     .ForMember(dest => dest.OrderStatus, opt => opt
            //         .MapFrom(src => src.Status)) 
            //     .ForMember(dest => dest.Tasks, opt => opt
            //         .MapFrom(src => src.Tasks));
            //
            //
            // CreateMap<Tasks, KanbanTaskResponse>();
            //
            // CreateMap<Order, OrderDetailsResponse>()
            //     .ForMember(dest => dest.Client, opt => opt
            //         .MapFrom(src => src.ClientOrders.FirstOrDefault().Client)) 
            //     .ForMember(dest => dest.Users, opt => opt
            //         .MapFrom(src => src.UserOrders.Select(uo => uo.User)));
            //
            // CreateMap<Client, ClientResponse>(); 
            // CreateMap<User, OrderDetailsUserResponse>()
            //     .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username));

        }
    }
}

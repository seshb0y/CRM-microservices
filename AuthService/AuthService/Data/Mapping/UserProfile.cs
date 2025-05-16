using AutoMapper;
using ControllerFirst.DTO.Requests;
using ControllerFirst.DTO.Responses;
using ControllerFirst.DTO.Responses.User;
using CRMSolution.Data.Models;
using CRMSolution.DTO.Requests;
using CRMSolution.Grpc.Users;
using Microsoft.AspNetCore.Identity.Data;
using RegisterRequest =  CRMSolution.Grpc.Users.RegisterRequest;
namespace CRMSolution.Data.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<RegisterRequest, User>()
            .ForMember(dest => dest.Username, opt =>
                opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt =>
                opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PasswordHash, opt =>
                opt.MapFrom(src => src.Password));

        CreateMap<User, CurrentUserResponse>()
            .ForMember(dest => dest.Id, opt => opt
                .MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt
                .MapFrom(src => src.Username))
            .ForMember(dest => dest.Role, opt => opt
                .MapFrom(src => src.Role))
            .ForMember(dest => dest.IsEmailConfirmed, opt => opt
                .MapFrom(src => src.IsEmailConfirmed))
            .ForMember(dest => dest.Email, opt => opt
                .MapFrom(src => src.Email));
        //
        // CreateMap<User, OrderDetailsUserResponse>()
        //     .ForMember(dest => dest.Username, opt => opt.
        //         MapFrom(src => src.Username));

        CreateMap<HttpCreateUserRequest, User>()
            .ForMember(dest => dest.Username, opt => opt
                .MapFrom(src => src.username))
            .ForMember(dest => dest.Email, opt => opt
                .MapFrom(src => src.email))
            .ForMember(dest => dest.PasswordHash, opt => opt
                .MapFrom(src => src.password));

        CreateMap<ChangeUserDataRequest, User>()
            .ForMember(dest => dest.Username, opt => opt
                .MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt => opt
                .MapFrom(src => src.NewEmail))
            .ForMember(dest => dest.Role, opt => opt
                .MapFrom(src => src.Role));

        CreateMap<User, ChangeUserDataResponse>()
            .ForMember(dest => dest.Username, opt => opt
                .MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt => opt
                .MapFrom(src => src.Email))
            .ForMember(dest => dest.Role, opt => opt
                .MapFrom(src => src.Role))
            .ForMember(dest => dest.CreatedAt, opt =>
                opt.MapFrom(src => Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(src.CreatedAt.ToUniversalTime())))
            .ForMember(dest => dest.Id, opt => opt
                .MapFrom(src => src.Id));

        CreateMap<User, GetUserResponse>()
            .ForMember(dest => dest.Id, opt => opt
                .MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt
                .MapFrom(src => src.Username))
            .ForMember(dest => dest.IsEmailConfirmed, opt => opt
                .MapFrom(src => src.IsEmailConfirmed))
            .ForMember(dest => dest.Role, opt => opt
                .MapFrom(src => src.Role))
            .ForMember(dest => dest.Email, opt => opt
                .MapFrom(src => src.Email));
        //     
        //     
        // CreateMap<User, GetAllUsersResponse>()
        //     .ForMember(dest => dest.UserId, opt => opt
        //         .MapFrom(src => src.Id))
        //     .ForMember(dest => dest.IsEmailConfirmed, opt => opt
        //         .MapFrom(src => src.IsEmailConfirmed))
        //     .ForMember(dest => dest.CreatedAt, opt => opt
        //         .MapFrom(src => src.CreatedAt))
        //     .ForMember(dest => dest.Username, opt => opt
        //         .MapFrom(src => src.Username))
        //     .ForMember(dest => dest.Email, opt => opt
        //         .MapFrom(src => src.Email))
        //     .ForMember(dest => dest.UserRole, opt => opt
        //         .MapFrom(src => src.Role))
        //     .ForMember(dest => dest.Tasks, opt => opt
        //         .MapFrom(src => 
        //         src.UserTasks != null 
        //             ? src.UserTasks.Select(t => new GetAllUsersTasksResponse
        //             {
        //                 TaskId = t.TaskId.ToString(),
        //                 TaskStatus = (TaskStatus)t.Task.Status
        //             }).ToList() 
        //             : new List<GetAllUsersTasksResponse>()
        //     ))
        //     .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => 
        //         src.UserOrders != null 
        //             ? src.UserOrders.Select(o => new GetAllUsersOrdersResponse
        //             {
        //                 OrderId = o.OrderId.ToString(),
        //                 OrderStatus = o.Order.Status
        //             }).ToList() 
        //             : new List<GetAllUsersOrdersResponse>()
        //     ))
        //     .ForMember(dest => dest.Clients, opt => opt.MapFrom(src => 
        //         src.ClientUsers != null 
        //             ? src.ClientUsers.Select(c => new GetAllUsersClientsResponse
        //             {
        //                 ClientName = c.Client.Name
        //             }).ToList()
        //             : new List<GetAllUsersClientsResponse>()
        //     ));
        //
        //
        // CreateMap<List<User>, GetAllUsersResponse>()
        //     .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src));

    }
}
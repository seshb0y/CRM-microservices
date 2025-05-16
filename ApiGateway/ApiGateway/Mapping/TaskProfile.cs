using ApiGateway.DTO.Responses;
using AutoMapper;
using CRMSolution.DTO.Requests.Task;
using CRMSolution.Grpc.Tasks;

namespace CRMSolution.Data.Mapping;

public class TaskProfile : Profile
{
    public TaskProfile()
    {
        // CreateMap<CreateTaskRequest, Tasks>()
        //     .ForMember(dest => dest.Title, opt => opt
        //         .MapFrom(src => src.title))
        //     .ForMember(dest => dest.Description, opt => opt
        //         .MapFrom(src => src.description))
        //     .ForMember(dest => dest.DueDate, opt => opt
        //         .MapFrom(src => src.endDate))
        //     .ForMember(dest => dest.OrderId, opt => opt
        //         .MapFrom(src => src.orderId));
        //
        // CreateMap<DeleteTaskRequest, Tasks>()
        //     .ForMember(dest => dest.Id, opt => opt
        //         .MapFrom(src => src.taskId));
        //
        // CreateMap<FindTaskRequest, Tasks>()
        //     .ForMember(dest => dest.Id, opt => opt
        //         .MapFrom(src => src.taskId));
        //
        // CreateMap<UpdateTaskRequest, Tasks>()
        //     .ForMember(dest => dest.Id, opt => opt
        //         .MapFrom(src => src.taskId))
        //     .ForMember(dest => dest.Description, opt => opt
        //         .MapFrom(src => src.description))
        //     .ForMember(dest => dest.Status, opt => opt
        //         .MapFrom(src => src.status));
        //
        // CreateMap<Tasks, TaskResponse>()
        //     .ForMember(dest => dest.Id, opt => opt
        //         .MapFrom(src => src.Id))
        //     .ForMember(dest => dest.Title, opt => opt
        //         .MapFrom(src => src.Title))
        //     .ForMember(dest => dest.Description, opt => opt
        //         .MapFrom(src => src.Description))
        //     .ForMember(dest => dest.UserTasks, opt => opt
        //         .MapFrom(src => src.UserTasks))
        //     .ForMember(dest => dest.Order, opt => opt
        //         .MapFrom(src => src.Order));
        //
        // CreateMap<UserTask, UserTaskResponse>()
        //     .ForMember(dest => dest.Id, opt => opt
        //         .MapFrom(src => src.UserId))
        //     .ForMember(dest => dest.Username, opt => opt
        //         .MapFrom(src => src.User.Username))
        //     .ForMember(dest => dest.Email, opt => opt
        //         .MapFrom(src => src.User.Email))
        //     .ForMember(dest => dest.CreatedAt, opt => opt
        //         .MapFrom(src => src.User.CreatedAt))
        //     .ForMember(dest => dest.IsEmailConfirmed, opt => opt
        //         .MapFrom(src => src.User.IsEmailConfirmed));
        CreateMap<TaskInfo, TaskDto>();

        CreateMap<GetAllTasksResponse, List<TaskDto>>()
            .ConvertUsing(src => src.Tasks.Select(t => new TaskDto
            {
                TaskId = t.Id,
                OrderId = t.OrderId,
                Title = t.Title,
                DueDate = t.DueDate.ToDateTime(),
                Status = t.Status ,
                Username = t.Username
            }).ToList());

    }
}
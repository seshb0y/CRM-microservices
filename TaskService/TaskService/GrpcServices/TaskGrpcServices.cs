using AutoMapper;
using CRMSolution.Data.Validators.Tasks;
using Grpc.Core;
using CRMSolution.Grpc.Tasks;
using FluentValidation;
using TaskService.Data.Models;
using TaskService.Data.Repository.TasksRep; // <-- Правильный namespace
using TaskService.Services.Interfaces;

namespace TaskService.GrpcServices;

public class TaskGrpcService : CRMSolution.Grpc.Tasks.TaskGrpcService.TaskGrpcServiceBase
{
    private readonly ITasksService _tasksService;
    private readonly ITasksRep _tasksRep;
    private readonly IValidator<CreateTaskRequest> _createTaskValidator;
    private readonly IValidator<UpdateTaskRequest> _updateTaskValidator;
    private readonly IValidator<DeleteTaskRequest> _deleteTaskValidator;
    private readonly IValidator<GetTaskByIdRequest> _getTaskByIdValidator;

    public TaskGrpcService(ITasksService tasksService, ITasksRep tasksRep,  IValidator<CreateTaskRequest> createTaskValidator, 
        IValidator<UpdateTaskRequest> updateTaskValidator,  IValidator<DeleteTaskRequest> deleteTaskValidator,
        IValidator<GetTaskByIdRequest> getTaskByIdValidator)
    {
        _tasksService = tasksService;
        _tasksRep = tasksRep;
        _createTaskValidator = createTaskValidator;
        _updateTaskValidator = updateTaskValidator;
        _deleteTaskValidator = deleteTaskValidator;
        _getTaskByIdValidator = getTaskByIdValidator;
    }

    public override async Task<GetTasksByUserIdsResponse> GetTasksByUserIds(GetTasksByUserIdsRequest request,
        ServerCallContext context)
    {
        return await _tasksService.GetTasksByUserIdsAsync(request);
    }

    public override async Task<GetTasksByOrderIdsResponse> GetTasksByOrderIds(GetTasksByOrderIdsRequest request,
        ServerCallContext context)
    {
        return await _tasksService.GetTasksByOrderIdsAsync(request);
    }
    public override async Task<GetTaskByIdResponse> GetTaskById(GetTaskByIdRequest request, ServerCallContext context)
    {
        var result = await _getTaskByIdValidator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = string.Join(" | ", result.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
        }
        var task = await _tasksService.GetByIdAsync(request.Id);

        return new GetTaskByIdResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = (int)task.Status
        };
    }
    
    public override async Task<CreateTaskResponse> CreateTask(CreateTaskRequest request, ServerCallContext context)
    {
        var result = await _createTaskValidator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = string.Join(" | ", result.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
        }
        return await _tasksService.CreateTaskAsync(request.OrderId, request.Description, request.DueDate.ToDateTime(), request.Title);
    }

    public override async Task<GetTaskByOrderIdResponse> GetTaskByOrderId(GetTaskByIdRequest request,
        ServerCallContext context)
    {
        return await _tasksService.GetTasksByOrderIdAsync(request.Id);
    }

    public override async Task<TaskInfo> UpdateTask(UpdateTaskRequest request,
        ServerCallContext context)
    {
        var result = await _updateTaskValidator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = string.Join(" | ", result.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
        }
        return await _tasksService.UpdateTaskAsync(request);
    }
    
    public override async Task<DeleteTaskResponse> DeleteTask(DeleteTaskRequest DeleteTaskRequest,
        ServerCallContext context)
    {
        var validator = new DeleteTaskValidator(_tasksRep);
        var result = await _deleteTaskValidator.ValidateAsync(DeleteTaskRequest);

        if (!result.IsValid)
        {
            var errorMessages = string.Join(" | ", result.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
        }
        return await _tasksService.DeleteTaskAsync(DeleteTaskRequest);
    }
    
    public override async Task<GetAllTasksResponse> GetAllTasks(GetAllTasksRequest getAllTasksRequest,
        ServerCallContext context)
    {
        return await _tasksService.GetAllTasks(getAllTasksRequest.Sort);
    }
}
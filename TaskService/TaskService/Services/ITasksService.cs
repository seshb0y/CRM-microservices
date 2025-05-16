using CRMSolution.Grpc.Tasks;
using TaskService.Data.Models;
using TaskService.DTO.Requests.Task;
using TaskService.DTO.Responses;
using UpdateTaskRequest = CRMSolution.Grpc.Tasks.UpdateTaskRequest;

namespace TaskService.Services.Interfaces;

public interface ITasksService
{
    public Task<CreateTaskResponse> CreateTaskAsync(int orderId, string description, DateTime dueDate, string title);
    public Task<TaskInfo> UpdateTaskAsync(UpdateTaskRequest request);
    public Task<DeleteTaskResponse> DeleteTaskAsync(DeleteTaskRequest request);
    public Task<GetAllTasksResponse> GetAllTasks(SortTasksRequest sortTasksRequest);
    Task<TaskEntity> GetByIdAsync(int id);
    Task<GetTaskByOrderIdResponse> GetTasksByOrderIdAsync(int orderId);
    Task<GetTasksByUserIdsResponse> GetTasksByUserIdsAsync(GetTasksByUserIdsRequest request);
    Task<GetTasksByOrderIdsResponse> GetTasksByOrderIdsAsync(GetTasksByOrderIdsRequest request);

}
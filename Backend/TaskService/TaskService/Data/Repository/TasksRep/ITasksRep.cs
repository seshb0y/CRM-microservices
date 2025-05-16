
using CRMSolution.Grpc.Tasks;
using TaskService.Data.Models;
using TaskService.Data.Repository.Interface;
using TaskService.DTO.Requests.Task;
using TaskService.DTO.Responses;

namespace TaskService.Data.Repository.TasksRep;

public interface ITasksRep : IRepository<TaskEntity>
{
    // public Task AddDependency(Order order, User user, Tasks task);

    public Task<TaskEntity> GetById(int taskId);
    
    Task<List<TaskDto>> GetLowInfoTasksList(SortTasksRequest sortTasksRequest);
    Task<List<TaskEntity>> GetTasksByOrderId(int orderId);
    Task<List<TaskEntity>> GetTasksByUserId(List<int> userIds);
    Task<List<TaskEntity>> GetTasksByOrderIds(List<int> orderIds);
}
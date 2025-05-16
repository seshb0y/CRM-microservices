using CRMSolution.Grpc.Tasks;
using TaskService.DTO.Responses;
using TaskService.Data.Models;
using TaskService.Data.Repository.Interface;
using TaskService.DTO.Requests.Task;
using Microsoft.EntityFrameworkCore;
using TaskService.Data.Models;
using TaskService.DTO.Requests.Task;
using TaskService.DTO.Responses;

namespace TaskService.Data.Repository.TasksRep;

public class TasksRep : Repository<TaskEntity>, ITasksRep
{
    public TasksRep(TaskDbContext context) : base(context)
    {
        
    }

    // public async Task AddDependency(Order order, User user, Tasks task)
    // {
    //     task.Order = order;
    //     task.UserTasks.Add(
    //         new UserTask
    //         {
    //             UserId = user.Id,
    //             TaskId = task.Id,
    //             User = user,
    //             Task = task
    //         });
    //     await AddAsync(task);
    // }

    // public async Task<Tasks> GetById(int taskId)
    // {
    //     // return await _context.Tasks
    //     //     .Include(t => t.Order)
    //     //     .Include(t => t.UserTasks)
    //     //     .ThenInclude(ut => ut.User)
    //     //     .FirstOrDefaultAsync(t => t.Id == taskId);
    //     throw new NotImplementedException();
    // }
    
    
    // public async Task<List<TaskDto>> GetLowInfoTasksList(SortTasksRequest sortTasksRequest)
    // {
    //     var query = _dbSet
    //         .Include(t => t.Order)
    //         .Include(t => t.UserTasks)
    //         .ThenInclude(ut => ut.User)
    //         .Select(t => new TaskDto
    //         {
    //             TaskId = t.Id,
    //             OrderId = t.Order.Id,
    //             Title = t.Title,
    //             Status = t.Status,
    //             DueDate = t.DueDate,
    //             Username = t.UserTasks.Select(ut => ut.User.Username).FirstOrDefault()
    //         });
    //
    //     query = sortTasksRequest.sortBy?.ToLower() switch
    //     {
    //         "taskid" => sortTasksRequest.Descending ? query.OrderByDescending(c => c.TaskId) : query.OrderBy(c => c.TaskId),
    //         "title" => sortTasksRequest.Descending ? query.OrderByDescending(c => c.Title) : query.OrderBy(c => c.Title),
    //         "status" => sortTasksRequest.Descending ? query.OrderByDescending(c => c.Status) : query.OrderBy(c => c.Status),
    //         "duedate" => sortTasksRequest.Descending ? query.OrderByDescending(c => c.DueDate) : query.OrderBy(c => c.DueDate),
    //         "username" => sortTasksRequest.Descending ? query.OrderByDescending(c => c.Username) : query.OrderBy(c => c.Username),
    //         _ => query
    //     };
    //
    //     return await query.ToListAsync();
    // }

    public async Task<TaskEntity> GetById(int taskId)
    {
        return await _context.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == taskId);
    }

    public Task<List<TaskDto>> GetLowInfoTasksList(SortTasksRequest sortTasksRequest)
    {
        throw new NotImplementedException();
    }

    public async Task<List<TaskEntity>> GetTasksByOrderId(int orderId)
    {
        return await _context.Tasks
            .Where(t => t.OrderId == orderId)
            .ToListAsync();
    }

    public async Task<List<TaskEntity>> GetTasksByUserId(List<int> userIds)
    {
        return await _context.Tasks
            .Where(t => userIds.Contains(t.UserId))
            .ToListAsync();
    }
    
    public async Task<List<TaskEntity>> GetTasksByOrderIds(List<int> orderIds)
    {
        return await _context.Tasks
            .Where(t => orderIds.Contains(t.OrderId))
            .ToListAsync();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskService.DTO.Requests.Task;
using TaskService.Services.Interfaces;

namespace TaskService.Controllers;

[ApiController]
[Route("api/v1/tasks/")]
public class TaskController : ControllerBase
{
    private readonly ITasksService _tasksService;

    public TaskController(ITasksService tasksService)
    {
        _tasksService = tasksService;
    }


    [HttpPost]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> AddTask([FromBody] HTTPCreateTaskRequest request)
    {
        // await _tasksService.CreateTaskAsync(request);
        return Ok("Task created");
    }
    
    [HttpPut]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> ChangeTask([FromBody] HttpUpdateTaskRequest request)
    {
        // await _tasksService.UpdateTaskAsync(request);
        return Ok("Task updated");
    }
    
    [HttpDelete]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> DeleteTask([FromBody] HttpDeleteTaskRequest request)
    {
        // await _tasksService.DeleteTaskAsync(request);
        return Ok("Task deleted");
    }
    
    [HttpGet("search")]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> FindTask([FromQuery] HttpFindTaskRequest request)
    {
        return Ok("await _tasksService.FindTaskByIdAsync(request)");
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllTasks([FromQuery] HttpSortTasksRequest httpSortTasksRequest)
    {
        // var orders = await _tasksService.GetAllTasks(sortTasksRequest);
        return Ok("orders");
    }
}
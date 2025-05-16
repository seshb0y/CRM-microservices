using ApiGateway.DTO.Requests;
using ApiGateway.Hubs;
using CRMSolution.DTO.Requests;
using CRMSolution.Grpc.Users;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UserRole = CRMSolution.Grpc.Orders.UserRole;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UserController : ControllerBase
{
    private readonly UserService.UserServiceClient _userService;
    private readonly IHubContext<NotificationHub> _notificationHub;

    public UserController(UserService.UserServiceClient userService,  IHubContext<NotificationHub> notificationHub)
    {
        _userService = userService;
        _notificationHub = notificationHub;
    }


    // [HttpPost("add")]
    // // [Authorize(Policy = "ManagerPolicy")]
    // public async Task<IActionResult> AddUser([FromBody] CreateUserRequest request)
    // {
    //     
    //     return Ok(await _userService.CreateUser(request));
    // }
    
    [HttpPut]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> ChangeUser([FromBody] HttpChangeUserDataRequest request)
    {
        // var validationResult = await validator.ValidateAsync(request);
        // if (!validationResult.IsValid)
        // {
        //     return BadRequest(validationResult.Errors);
        // }
        var grpcRequest = new ChangeUserDataRequest
        {
            NewEmail = request.newEmail,
            OldEmail = request.oldEmail,
            Role = request.role,
            Username = request.username,
        };
        var grpcResponse = await _userService.ChangeUserDataAsync(grpcRequest);
        
        await _notificationHub.Clients.All.SendAsync("UserUpdated", new
        {
            grpcResponse.Id,
            grpcResponse.Email,
            grpcResponse.Username,
            grpcResponse.CreatedAt,
            grpcResponse.Role,
        });
        
        return Ok(grpcResponse);
    }
    
    [HttpDelete]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> DeleteUser([FromBody] HttpDeleteUserRequest request)
    {
        var grpcRequest = new DeleteUserRequest
        {
            Email = request.email,
        };
        var grpcResponse = await _userService.DeleteUserAsync(grpcRequest);
        
        await _notificationHub.Clients.All.SendAsync("UserDeleted", new
        {
            grpcResponse.UserId,
        });
        
        return Ok("User deleted");
    }
    
    [HttpGet("search")]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> LoadUserData([FromQuery] HttpFindUserRequest request)
    {
        var grpcRequest = new GetUserByEmailRequest
        {
            Email = request.email,
        };
        var grpcResponse = await _userService.FindUserAsync(grpcRequest);
        return Ok(grpcResponse);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers([FromQuery] SortUsersRequest sortUsersRequest)
    {
        var grpcRequest = new GetAllUsersRequest
        {
            Sort = new SortUsersRequest(sortUsersRequest)
        };
        var grpcResponse = await _userService.GetAllUsersAsync(grpcRequest);
        
        var users = grpcResponse.Users.Select(u => new HttpUserWithDetailsResponse
        {
            UserId = u.UserId,
            Username = u.Username,
            Email = u.Email,
            UserRole = (UserRole)u.UserRole,
            IsEmailConfirmed = u.IsEmailConfirmed,
            CreatedAt = u.CreatedAt.ToDateTime(),

            Tasks = u.Tasks.Select(t => new HttpTaskInfo
            {
                TaskId = t.TaskId,
                Title = t.Title,
                Description = t.Description,
                TaskStatus = t.TaskStatus.ToString(),
                DueDate = t.DueDate.ToDateTime()
            }).ToList(),

            Orders = u.Orders.Select(o => new HttpOrderInfo
            {
                OrderId = o.OrderId,
                TotalAmount = o.TotalAmount,
                OrderStatus = o.OrderStatus.ToString()
            }).ToList()
        });

        var response = new HttpGetAllUsersResponse
        {
            Users = users.ToList()
        };
        return Ok(response);
    }
    //
    // [HttpGet("Get/Clients/With/Orders/And/Tasks")]
    // public async Task<IActionResult> GetClientsWithOrdersAndTasks()
    // {
    //     var clients = await _clientService.GetClientsWithOrdersAndTasks(HttpContext);
    //     return Ok(clients);
    // }



}
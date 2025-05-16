using System.Runtime.InteropServices.JavaScript;
using ControllerFirst.DTO.Responses.User;
using CRMSolution.Data.Models;

public class HttpGetAllUsersResponse
{
     public List<HttpFindUserReponse> Users { get; set; }
}

// public class GetAllUsersUserResponse
// {
//     public string UserId { get; set; }
//     public string Username { get; set; }
//     public string Email { get; set; }
//     
//     public bool IsEmailConfirmed { get; set; }
//     public UserRole UserRole { get; set; }
//     public DateTime CreatedAt { get; set; }
//     public List<GetAllUsersTasksResponse> Tasks { get; set; }
//     public List<GetAllUsersOrdersResponse> Orders { get; set; }
//     public List<GetAllUsersClientsResponse> Clients { get; set; }
// }
//
// public class GetAllUsersTasksResponse
// {
//     public string TaskId { get; set; }
//     public TaskStatus TaskStatus { get; set; }
// }
//
// public class GetAllUsersOrdersResponse
// {
//     public string OrderId { get; set; }
//     public OrderStatus OrderStatus { get; set; }
// }
//
// public class GetAllUsersClientsResponse
// {
//     public string ClientName { get; set; }
// }
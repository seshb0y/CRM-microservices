using System.Runtime.InteropServices.JavaScript;
using CRMSolution.Grpc.Orders;
using CRMSolution.Grpc.Users;
using OrderStatus = CRMSolution.Grpc.Users.OrderStatus;
using UserRole = CRMSolution.Grpc.Orders.UserRole;

public class HttpGetAllUsersResponse
{
    public List<HttpUserWithDetailsResponse> Users { get; set; } = new();
}

public class HttpUserWithDetailsResponse
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole UserRole { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<HttpTaskInfo> Tasks { get; set; } = new();
    public List<HttpOrderInfo> Orders { get; set; } = new();
}

public class HttpTaskInfo
{
    public int TaskId { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string TaskStatus { get; set; } = "";
    public DateTime DueDate { get; set; }
}

public class HttpOrderInfo
{
    public int OrderId { get; set; }
    public double TotalAmount { get; set; }
    public string OrderStatus { get; set; } = "";
}

using OrderService.Data.Models;

namespace OrderService.DTO.Responses;

public class OrderDetailsResponse
{
    public int Id { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> CallRecordingUrl { get; set; }

    public ClientResponse Client { get; set; }
    public List<OrderDetailsTaskResponse> Tasks { get; set; } = new();
    public List<OrderDetailsUserResponse> Users { get; set; } = new();
}

public class ClientResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
}

public class OrderDetailsTaskResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public TaskStatus Status { get; set; }
    public DateTime DueDate { get; set; }
}

public class OrderDetailsUserResponse
{
    public string Username { get; set; }
}
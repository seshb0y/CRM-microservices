namespace ClientService.DTO.Responses;

public class HttpClientWithOrdersAndTasksResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<KanbanOrderResponse> Orders { get; set; } = new();
}

public class KanbanOrderResponse
{
    public int Id { get; set; }
    public double TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<TaskResponse> Tasks { get; set; } = new();
}

public class TaskResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
}
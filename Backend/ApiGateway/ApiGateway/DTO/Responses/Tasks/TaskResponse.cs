namespace ApiGateway.DTO.Responses;


public class TaskResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public OrderResponse Order { get; set; }
    public List<UserTaskResponse> UserTasks { get; set; }
}


public class UserTaskResponse
{
    public int Id { get; set; }
    public string Username { get; set; } 
    public string Email { get; set; } = string.Empty;
    public bool IsEmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
namespace ApiGateway.DTO.Responses;

public class ClientWithOrdersAndTasksResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } 
    public List<KanbanOrderResponse> Orders { get; set; } = new();
}
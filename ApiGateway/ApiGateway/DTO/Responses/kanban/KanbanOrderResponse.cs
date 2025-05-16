namespace ApiGateway.DTO.Responses;

public class KanbanOrderResponse
{
    public int OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public string OrderStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<KanbanTaskResponse> Tasks { get; set; } = new();
}
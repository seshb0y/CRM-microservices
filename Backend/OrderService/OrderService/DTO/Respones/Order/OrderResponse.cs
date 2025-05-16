namespace OrderService.DTO.Responses;

public class ClientOrderDto
{
    public Guid ClientId { get; set; }
    public string ClientName { get; set; }
    public string ClientEmail { get; set; }
    public string ClientPhone { get; set; }
    public string CreatedAt { get; set; }
    public string ClientAddress { get; set; }
}

public class OrderResponse
{
    public int Id { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<ClientOrderDto> ClientOrders { get; set; } = new();
}
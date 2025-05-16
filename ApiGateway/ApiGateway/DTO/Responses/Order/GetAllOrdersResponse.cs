using CRMSolution.Grpc.Orders;

namespace ApiGateway.DTO.Responses;

public class GetAllOrdersResponse
{
    public List<OrderDTO> Orders { get; set; }
}

public class OrderDTO
{
    public int Id { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Username { get; set; }
    public string ClientName { get; set; }
}
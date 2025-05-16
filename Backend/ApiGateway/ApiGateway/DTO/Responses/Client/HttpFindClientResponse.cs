using CRMSolution.Grpc.Orders;

namespace ApiGateway.DTO.Responses;

public class HttpFindClientResponse
{
    public OrderDto[]? Orders { get; set; }
    public UserDto[]? Users { get; set; }
}


public class OrderDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public OrderStatus Status { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
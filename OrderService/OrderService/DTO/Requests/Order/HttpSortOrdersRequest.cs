namespace OrderService.DTO.Requests.Orders;

public class HttpSortOrdersRequest
{
    public string? sortBy { get; set; } 
    public bool Descending { get; set; } = false; 
}
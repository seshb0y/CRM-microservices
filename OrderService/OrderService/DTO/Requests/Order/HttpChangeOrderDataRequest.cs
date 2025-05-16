using OrderService.Data.Models;

namespace OrderService.DTO.Requests;

public record HttpChangeOrderDataRequest(decimal totalAmount, OrderStatus status, int orderId);
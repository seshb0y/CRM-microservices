using CRMSolution.Grpc.Orders;

namespace ApiGateway.DTO.Requests;

public record HttpChangeOrderDataRequest(decimal totalAmount, OrderStatus status, int orderId);
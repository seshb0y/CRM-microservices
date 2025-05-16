namespace ApiGateway.DTO.Requests;

public record HttpCreateOrderRequest(decimal totalAmount, string clientEmail, string userEmail);
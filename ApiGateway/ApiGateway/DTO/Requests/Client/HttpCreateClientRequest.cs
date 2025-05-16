namespace ApiGateway.DTO.Requests.Client;

public record HttpCreateClientRequest(string name, string email, string phone, string address);
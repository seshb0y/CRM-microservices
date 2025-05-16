namespace ApiGateway.DTO.Requests.Client;

public record HttpChangeDataClientRequest(string name, string newEmail, string phone, string address, string oldEmail);
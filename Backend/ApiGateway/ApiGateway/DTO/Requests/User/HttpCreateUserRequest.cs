namespace ApiGateway.DTO.Requests;

public record HttpCreateUserRequest(string username, string password, string email);
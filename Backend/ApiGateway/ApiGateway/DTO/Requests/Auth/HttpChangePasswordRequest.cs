namespace ApiGateway.DTO.Requests;

public record HttpChangePasswordRequest(string newPassword, string token);
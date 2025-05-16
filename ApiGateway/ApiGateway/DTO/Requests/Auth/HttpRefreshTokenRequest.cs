namespace ApiGateway.DTO.Requests;

public record HttpRefreshTokenRequest(string username, string refreshToken);


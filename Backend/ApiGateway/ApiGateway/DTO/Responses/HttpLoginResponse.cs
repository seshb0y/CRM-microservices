namespace ApiGateway.DTO.Responses;

public record HttpLoginResponse(string accessToken, string refreshToken);
namespace ApiGateway.DTO.Responses;

public record HttpRefreshTokenResponse(string accessToken, string refreshToken);
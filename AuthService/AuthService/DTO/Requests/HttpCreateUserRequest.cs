namespace CRMSolution.DTO.Requests;

public record HttpCreateUserRequest(string username, string password, string email);
using CRMSolution.Grpc.Users;

namespace ApiGateway.DTO.Responses;

public class GetCurrentUserResponse
{
    public int Id { get; set; }
    public string Username { get; set; }
    public UserRole Role { get; set; }
    
    public bool IsEmailConfirmed { get; set; }
        
    public string Email { get; set; }
}
using CRMSolution.Data.Models;

namespace ControllerFirst.DTO.Responses;

public class HttpGetCurrentUserResponse
{
    public int Id { get; set; }
    public string Username { get; set; }
    public UserRole Role { get; set; }
    
    public bool IsEmailConfirmed { get; set; }
        
    public string Email { get; set; }
}
namespace CRMSolution.DTO.Requests;

public class HttpSortUsersRequest
{
    public string? sortBy { get; set; } 
    public bool Descending { get; set; } = false; 
}
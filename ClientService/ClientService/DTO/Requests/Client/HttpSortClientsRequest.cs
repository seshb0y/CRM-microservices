namespace ClientService.DTO.Requests.Client;

public class HttpSortClientsRequest
{
    public string? sortBy { get; set; } 
    public bool Descending { get; set; } = false; 
}
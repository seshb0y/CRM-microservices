using ClientService.Data.Models;

namespace ClientService.DTO.Responses;

public class HttpGetAllClientsResponse
{
    public List<Client> Clients { get; set; }
}
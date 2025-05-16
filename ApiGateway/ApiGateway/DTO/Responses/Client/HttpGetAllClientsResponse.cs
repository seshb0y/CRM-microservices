using ApiGateway.DTO.MappingDto;

namespace ApiGateway.DTO.Responses;

public class HttpGetAllClientsResponse
{
    public List<ClientDto> Clients { get; set; }
}
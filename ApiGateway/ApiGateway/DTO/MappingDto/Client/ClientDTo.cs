namespace ApiGateway.DTO.MappingDto;

public class ClientDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public int OrderId { get; set; }
    public int Id { get; set; }
}
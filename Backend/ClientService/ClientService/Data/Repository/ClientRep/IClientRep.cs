using ClientService.Data.Models;
using ClientService.Data.Repository.Interface;
using ClientService.DTO.Requests.Client;
using ClientService.DTO.Responses;
using ClientService.DTO.Responses;
using ClientService.Data.Models;
using ClientService.Data.Repository.Interface;
using CRMSolution.DTO.Requests.Client;

namespace ClientService.Data.Repository.SpecialRepClass.ClientRep;

public interface IClientRep : IRepository<Client>
{
    Task <IEnumerable<Client?>> GetClientsByManagerIdAsync(int managerId);
    Task<Client?> GetClientByEmail(String email);
    
    Task<HttpFindClientResponse> GetClientsOrdersAndUsersAsync(string email);
    
    Task<Client?> GetClientByName(string name);
    Task<List<Client>> GetLowInfoClientsList(HttpSortClientsRequest httpSortClientsRequest);
    Task<List<Client>> GetClientsByIdsAsync(List<int> ids);
    
    
    // Task<List<Order>> GetOrdersByUsername(string username);

    // Task<List<Client>> GetClientsByOrdersAsync(List<Order> orders);

}
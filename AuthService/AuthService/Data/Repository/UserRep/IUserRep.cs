using ControllerFirst.DTO.Responses.User;
using CRMSolution.Data.Models;
using CRMSolution.Data.Repository.Interface;
using CRMSolution.DTO.Requests;

namespace CRMSolution.Data.Repository.UserRep;

public interface IUserRep : IRepository<User>
{
    public Task<User?> FindByNameAsync(string name);
    public Task<User?> FindByEmailAsync(string name);
    
    Task<List<User>> GetUsersByIdsAsync(List<int> ids);

    
    public Task SaveChangesAsync();

    // public Task<FindUserReponse> GetUsersTasksOrdersClientsAsync(string email);
    //
    // Task<List<User>> GetLowInfoUsersList(SortUsersRequest sortUsersRequest);
}
using AuthService.Data;
using ControllerFirst.DTO.Responses.User;
using CRMSolution.Data.Models;
using CRMSolution.Data.Repository.Interface;
using CRMSolution.DTO.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace CRMSolution.Data.Repository.UserRep;

public class UserRep : Repository<User>, IUserRep
{
    public UserRep(AuthDbContext context) : base(context)
    {
        
    }
    
    public async Task<User?> FindByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Username == name);
    }
    
    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }
    
    public Task<List<User>> GetUsersByIdsAsync(List<int> ids)
    {
        return _context.Users
            .Where(u => ids.Contains(u.Id))
            .ToListAsync();
    }

    
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    
    // public async Task<FindUserReponse> GetUsersTasksOrdersClientsAsync(string email)
    // {
    //     var user = await _dbSet
    //         .Where(u => u.Email == email)
    //         .Include(c => c.UserOrders)
    //         .ThenInclude(uo => uo.Order)
    //         .Include(c => c.ClientUsers)
    //         .ThenInclude(uc => uc.Client)
    //         .Include(c => c.UserTasks)
    //         .ThenInclude(ut => ut.Task)
    //         .FirstOrDefaultAsync();
    //
    //     if (user == null) return null;
    //
    //     return new FindUserReponse
    //     {
    //         orders = user.UserOrders.Select(uo => new FindUserOrdersResponse()
    //         {
    //             orderId = uo.Order.Id.ToString(),
    //             totalAmount = uo.Order.TotalAmount,
    //             status = uo.Order.Status.ToString()
    //         }).ToArray(),
    //
    //         clients = user.ClientUsers.Select(uc => new FindUserClientsResponse()
    //         {
    //             clientName = uc.Client.Name,
    //         }).ToArray(),
    //         
    //         tasks = user.UserTasks.Select(ut => new FindUserTasksResponse()
    //         {
    //             taskId = ut.Task.Id.ToString(),
    //             title = ut.Task.Title,
    //             status = ut.Task.Status.ToString()
    //         }).ToArray()
    //     };
    // }
    //
    // public async Task<List<User>> GetLowInfoUsersList(SortUsersRequest sortUsersRequest)
    // {
    //     var query = _dbSet
    //         .Include(u => u.UserTasks)
    //         .ThenInclude(ut => ut.Task)
    //         .Include(u => u.UserOrders)
    //         .ThenInclude(uo => uo.Order)
    //         .Include(u => u.ClientUsers)
    //         .ThenInclude(uc => uc.Client)
    //         .Select(c => new User()
    //         {
    //             Id = c.Id,
    //             Username = c.Username,
    //             Email = c.Email,
    //             IsEmailConfirmed = c.IsEmailConfirmed,
    //             Role = c.Role,
    //             CreatedAt = c.CreatedAt,
    //             UserTasks = c.UserTasks, 
    //             UserOrders = c.UserOrders,
    //             ClientUsers = c.ClientUsers
    //         });
    //
    //     
    //     query = sortUsersRequest.sortBy?.ToLower() switch
    //     {
    //         "id" => sortUsersRequest.Descending ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
    //         "username" =>sortUsersRequest.Descending ? query.OrderByDescending(c => c.Username) :  query.OrderBy(c => c.Username),
    //         "email" => sortUsersRequest.Descending ? query.OrderByDescending(c => c.Email) : query.OrderBy(c => c.Email),
    //         "role" => sortUsersRequest.Descending ? query.OrderByDescending(c => c.Role) : query.OrderBy(c => c.Role),
    //         "isemailconfirmed" => sortUsersRequest.Descending ? query.OrderByDescending(c => c.IsEmailConfirmed) : query.OrderBy(c => c.IsEmailConfirmed),
    //         "createdat" => sortUsersRequest.Descending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
    //         _ => query
    //     };
    //
    //     return await query.ToListAsync();
    // }
}
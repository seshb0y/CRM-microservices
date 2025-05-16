namespace OrderService.Data.Repository.Interface;

public interface IRepository<T> where T : class
{
    Task<T> GetById(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync (T entity);
    void Update (T entity);
    void Delete (T entity);
    Task SaveChangesAsync();
}
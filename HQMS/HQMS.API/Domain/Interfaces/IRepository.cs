namespace HQMS.API.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        Task<int> UpdateAsync(T entity); // Changed from void to Task<int>
        Task<int> DeleteAsync(Guid id); // Changed from void to Task<int>
    }
}

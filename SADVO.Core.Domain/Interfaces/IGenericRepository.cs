
namespace Sadvo.Core.Domain.Interfaces
{
    public interface IGenericRepository<Entity> where Entity : class
    {
        Task<Entity?> AddAsync(Entity entity);
        Task<Entity?> UpdateAsync(int id, Entity entity);
        Task DeleteAsync(int id);
        Task<Entity?> GetByIdAsync(int id);
        Task<List<Entity>> GetAllList();
        IQueryable<Entity> GetAllQueryable();
        Task<List<Entity>?> AddRangeAsync(List<Entity> entities);
        IQueryable<Entity> GetAllQueryWithInclude(List<string> properties);
        Task<List<Entity>> GetAllListWithInclude(List<string> properties);
    }
}

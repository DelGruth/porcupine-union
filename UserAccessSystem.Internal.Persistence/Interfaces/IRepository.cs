using System.Linq.Expressions;
using UserAccessSystem.Domain.Common;

namespace UserAccessSystem.Internal.Persistence.Interfaces;

public interface IRepository<T>
    where T : BaseDomainObj
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
    Task UpdateAsync(T entity, Guid editedById);
    Task DeleteAsync(T entity, Guid editedById);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    IQueryable<T> Query();
}

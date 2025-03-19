using System.Linq.Expressions;
using UserAccessSystem.Contract;
using UserAccessSystem.Domain.Common;

namespace UserAccessSystem.Internal.Persistence.Interfaces;

public interface IRepository<T>
    where T : BaseDomainObj
{
    Task<Response<T>> GetByIdAsync(Guid id);
    Task<Response<IEnumerable<T>>> GetAllAsync();
    Task<Response<IReadOnlyList<T>>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<Response<T>> AddAsync(T entity);
    Task<Response<IEnumerable<T>>> AddRangeAsync(IEnumerable<T> entities);
    Task<Response<bool>> UpdateAsync(T entity);
    Task<Response<bool>> DeleteAsync(T entity);
}

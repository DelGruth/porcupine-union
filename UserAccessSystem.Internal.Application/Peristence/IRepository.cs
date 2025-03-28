using System.Linq.Expressions;
using UserAccessSystem.Contract;
using UserAccessSystem.Domain.Common;

namespace UserAccessSystem.Internal.Application.Peristence;

public interface IRepository<T>
    where T : BaseDomainObj
{
    Task<Response<T>> GetByIdAsync(Guid id, CancellationToken ctx = default);
    Task<Response<IEnumerable<T>>> GetAllAsync(CancellationToken ctx = default);
    Task<Response<IReadOnlyList<T>>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ctx = default
    );
    Task<Response<T>> AddAsync(T entity, CancellationToken ctx = default);
    Task<Response<IEnumerable<T>>> AddRangeAsync(
        IEnumerable<T> entities,
        CancellationToken ctx = default
    );
    Task<Response<bool>> UpdateAsync(T entity, CancellationToken ctx = default);
    Task<Response<bool>> DeleteAsync(T entity, CancellationToken ctx = default);
    Task<Response<bool>> DeleteAsync(Guid id, CancellationToken ctx = default);
}

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using UserAccessSystem.Contract;
using UserAccessSystem.Domain.Common;
using UserAccessSystem.Internal.Persistence.DbContext;
using UserAccessSystem.Internal.Persistence.Interfaces;

namespace UserAccessSystem.Internal.Persistence.Repository;

public class Repository<T> : IRepository<T>
    where T : BaseDomainObj
{
    protected readonly UserAccessDbContext _dbContext;

    public Repository(UserAccessDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<Response<T>> AddAsync(T entity)
    {
        try
        {
            entity.CreatedAtDateTime = DateTime.UtcNow;
            entity.EditedDateTime = DateTime.UtcNow;
            entity.Version = 1;
            entity.IsDeleted = false;

            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return new Response<T>(entity);
        }
        catch (Exception ex)
        {
            return new Response<T>(
                ErrorCode.UnexpectedError,
                $"Failed to add entity: {ex.Message}"
            );
        }
    }

    public async Task<Response<IEnumerable<T>>> AddRangeAsync(IEnumerable<T> entities)
    {
        try
        {
            var now = DateTime.UtcNow;

            foreach (var entity in entities)
            {
                entity.CreatedAtDateTime = now;
                entity.EditedDateTime = now;
                entity.Version = 1;
                entity.IsDeleted = false;
            }

            await _dbContext.Set<T>().AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();

            return new Response<IEnumerable<T>>(entities);
        }
        catch (Exception ex)
        {
            return new Response<IEnumerable<T>>(
                ErrorCode.UnexpectedError,
                $"Failed to add entities: {ex.Message}"
            );
        }
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbContext.Set<T>().Where(predicate).Where(e => !e.IsDeleted).CountAsync();
    }

    public async Task<Response<bool>> DeleteAsync(T entity)
    {
        try
        {
            entity.IsDeleted = true;
            entity.EditedDateTime = DateTime.UtcNow;
            entity.Version++;

            _dbContext.Set<T>().Update(entity);
            await _dbContext.SaveChangesAsync();

            return new Response<bool>(true);
        }
        catch (Exception ex)
        {
            return new Response<bool>(
                ErrorCode.UnexpectedError,
                $"Failed to delete entity: {ex.Message}"
            );
        }
    }

    public async Task<Response<IReadOnlyList<T>>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        try
        {
            var entities = await _dbContext
                .Set<T>()
                .Where(predicate)
                .Where(e => !e.IsDeleted)
                .ToListAsync();

            return new Response<IReadOnlyList<T>>(entities);
        }
        catch (Exception ex)
        {
            return new Response<IReadOnlyList<T>>(
                ErrorCode.UnexpectedError,
                $"Error finding entities: {ex.Message}"
            );
        }
    }

    public async Task<Response<IEnumerable<T>>> GetAllAsync()
    {
        try
        {
            var entities = await _dbContext.Set<T>().Where(e => !e.IsDeleted).ToListAsync();

            return new Response<IEnumerable<T>>(entities);
        }
        catch (Exception ex)
        {
            return new Response<IEnumerable<T>>(
                ErrorCode.UnexpectedError,
                $"Error retrieving all entities: {ex.Message}"
            );
        }
    }

    public async Task<Response<T>> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _dbContext
                .Set<T>()
                .Where(e => e.Id == id)
                .Where(e => !e.IsDeleted)
                .FirstOrDefaultAsync();

            return entity == null
                ? new Response<T>(ErrorCode.NotFound, $"Entity with ID {id} not found")
                : new Response<T>(entity);
        }
        catch (Exception ex)
        {
            return new Response<T>(
                ErrorCode.UnexpectedError,
                $"Error retrieving entity: {ex.Message}"
            );
        }
    }

    public async Task<Response<bool>> UpdateAsync(T entity)
    {
        try
        {
            entity.EditedDateTime = DateTime.UtcNow;
            entity.Version++;

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return new Response<bool>(true);
        }
        catch (DbUpdateConcurrencyException)
        {
            return new Response<bool>(
                ErrorCode.UnexpectedError,
                "The entity has been modified by another user"
            );
        }
        catch (Exception ex)
        {
            return new Response<bool>(
                ErrorCode.UnexpectedError,
                $"Failed to update entity: {ex.Message}"
            );
        }
    }
}

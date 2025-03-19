using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using UserAccessSystem.Domain.Common;
using UserAccessSystem.Internal.Persistence.DbContext;
using UserAccessSystem.Internal.Persistence.Interfaces;

namespace UserAccessSystem.Internal.Persistence.Repository;

public class Repository<T>(UserAccessDbContext dbContext) : IRepository<T>
    where T : BaseDomainObj?
{
    public async Task<T> AddAsync(T entity)
    {
        entity.CreatedAtDateTime = DateTime.UtcNow;
        entity.EditedDateTime = DateTime.UtcNow;
        entity.Version = 1;
        entity.IsDeleted = false;

        await dbContext.Set<T>().AddAsync(entity);
        await dbContext.SaveChangesAsync();

        return entity;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
    {
        var now = DateTime.UtcNow;

        var baseDomainObjs = entities.ToList();
        foreach (var entity in baseDomainObjs)
        {
            entity.CreatedAtDateTime = now;
            entity.EditedDateTime = now;
            entity.Version = 1;
            entity.IsDeleted = false;
        }

        await dbContext.Set<T>().AddRangeAsync(baseDomainObjs);
        await dbContext.SaveChangesAsync();

        return baseDomainObjs;
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return await dbContext.Set<T>().Where(e => !e.IsDeleted).Where(predicate).CountAsync();
    }

    public async Task DeleteAsync(T entity, Guid editedById)
    {
        entity.IsDeleted = true;
        entity.EditedDateTime = DateTime.UtcNow;
        entity.Version++;
        entity.EditedById = editedById;

        dbContext.Set<T>().Update(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await dbContext.Set<T>().Where(e => !e.IsDeleted).AnyAsync(predicate);
    }

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await dbContext.Set<T>().Where(e => !e.IsDeleted).Where(predicate).ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await dbContext.Set<T>().Where(e => !e.IsDeleted).ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await dbContext
            .Set<T>()
            .Where(e => !e.IsDeleted)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public IQueryable<T> Query()
    {
        return dbContext.Set<T>().Where(e => !e.IsDeleted);
    }

    public async Task UpdateAsync(T entity, Guid editedById)
    {
        entity.EditedDateTime = DateTime.UtcNow;
        entity.Version++;
        entity.EditedById = editedById;

        dbContext.Entry(entity).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();
    }
}

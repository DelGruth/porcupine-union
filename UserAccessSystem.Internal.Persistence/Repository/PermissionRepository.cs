using Microsoft.EntityFrameworkCore;
using UserAccessSystem.Contract;
using UserAccessSystem.Domain.Permissions;
using UserAccessSystem.Internal.Application.Peristence;
using UserAccessSystem.Internal.Persistence.DbContext;

namespace UserAccessSystem.Internal.Persistence.Repository;

public class PermissionRepository(UserAccessDbContext dbContext)
    : Repository<Permission>(dbContext),
        IPermissionRepository
{
    private readonly UserAccessDbContext dbContext = dbContext;

    public async Task<Response<IEnumerable<Permission>>> GetAllAsync(
        CancellationToken ctx = default
    )
    {
        try
        {
            var permissions = await dbContext
                .Permissions.Where(p => !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync(ctx);

            return new Response<IEnumerable<Permission>>(permissions);
        }
        catch (Exception ex)
        {
            return new Response<IEnumerable<Permission>>(
                ErrorCode.UnexpectedError,
                $"Error retrieving permissions: {ex.Message}"
            );
        }
    }

    public async Task<Response<Permission>> GetByIdAsync(Guid id, CancellationToken ctx = default)
    {
        try
        {
            var permission = await dbContext
                .Permissions.Where(p => p.Id == id)
                .Where(p => !p.IsDeleted)
                .FirstOrDefaultAsync(ctx);

            return permission == null
                ? new Response<Permission>(ErrorCode.NotFound, $"Permission with ID {id} not found")
                : new Response<Permission>(permission);
        }
        catch (Exception ex)
        {
            return new Response<Permission>(
                ErrorCode.UnexpectedError,
                $"Error retrieving permission: {ex.Message}"
            );
        }
    }

    public async Task<Response<Permission>> CreateAsync(
        Permission permission,
        CancellationToken ctx = default
    )
    {
        try
        {
            permission.Id = Guid.NewGuid();
            permission.CreatedAtDateTime = DateTime.UtcNow;
            permission.IsDeleted = false;

            await dbContext.Permissions.AddAsync(permission, ctx);
            await dbContext.SaveChangesAsync(ctx);

            return new Response<Permission>(permission);
        }
        catch (Exception ex)
        {
            return new Response<Permission>(
                ErrorCode.UnexpectedError,
                $"Error creating permission: {ex.Message}"
            );
        }
    }

    public async Task<Response<bool>> DeleteAsync(Guid id, CancellationToken ctx = default)
    {
        try
        {
            var permission = await dbContext.Permissions.FirstOrDefaultAsync(
                p => p.Id == id && !p.IsDeleted,
                ctx
            );

            if (permission == null)
                return new Response<bool>(ErrorCode.NotFound, $"Permission with ID {id} not found");

            permission.IsDeleted = true;
            permission.EditedDateTime = DateTime.UtcNow;

            dbContext.Permissions.Update(permission);
            await dbContext.SaveChangesAsync(ctx);

            return new Response<bool>(true);
        }
        catch (Exception ex)
        {
            return new Response<bool>(
                ErrorCode.UnexpectedError,
                $"Error deleting permission: {ex.Message}"
            );
        }
    }

    public async Task<Response<IEnumerable<Permission>>> GetPermissionsByGroupIdAsync(
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        try
        {
            var permissions = await dbContext
                .Permissions.Where(p => p.GroupPermissions.Any(gp => gp.GroupId == groupId))
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync(ctx);

            return new Response<IEnumerable<Permission>>(permissions);
        }
        catch (Exception ex)
        {
            return new Response<IEnumerable<Permission>>(
                ErrorCode.UnexpectedError,
                $"Error retrieving group permissions: {ex.Message}"
            );
        }
    }

    public async Task<Response<IEnumerable<Permission>>> GetPermissionsByUserIdAsync(
        Guid userId,
        CancellationToken ctx = default
    )
    {
        try
        {
            var permissions = await dbContext
                .Permissions.Where(p => !p.IsDeleted)
                .Where(p =>
                    p.UserPermissions.Any(up => up.UserId == userId && !up.IsDeleted)
                    || p.GroupPermissions.Any(gp =>
                        !gp.IsDeleted && gp.Group.Users.Any(u => u.UserId == userId && !u.IsDeleted)
                    )
                )
                .OrderBy(p => p.Name)
                .ToListAsync(ctx);

            return new Response<IEnumerable<Permission>>(permissions);
        }
        catch (Exception ex)
        {
            return new Response<IEnumerable<Permission>>(
                ErrorCode.UnexpectedError,
                $"Error retrieving user permissions: {ex.Message}"
            );
        }
    }
}

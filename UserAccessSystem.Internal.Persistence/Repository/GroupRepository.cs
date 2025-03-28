using Microsoft.EntityFrameworkCore;
using UserAccessSystem.Contract;
using UserAccessSystem.Domain.Group;
using UserAccessSystem.Domain.Permissions;
using UserAccessSystem.Domain.User;
using UserAccessSystem.Internal.Application.Peristence;
using UserAccessSystem.Internal.Persistence.Common;
using UserAccessSystem.Internal.Persistence.DbContext;

namespace UserAccessSystem.Internal.Persistence.Repository;

public class GroupRepository(UserAccessDbContext dbContext)
    : Repository<Group>(dbContext),
        IGroupRepository
{
    private readonly UserAccessDbContext dbContext = dbContext;

    public async Task<Response<IEnumerable<Group>>> GetAllAsync(CancellationToken ctx = default)
    {
        try
        {
            var groups = await dbContext
                .Groups.Include(g => g.GroupPermissions.Where(gp => !gp.IsDeleted))
                .ThenInclude(gp => gp.Permission)
                .Include(g => g.Users.Where(u => !u.IsDeleted))
                .ThenInclude(u => u.User)
                .Include(g => g.MemberPermissions.Where(mp => !mp.IsDeleted))
                .ThenInclude(mp => mp.Permission)
                .Where(g => !g.IsDeleted)
                .OrderBy(g => g.Name)
                .ToListAsync(ctx);

            return new Response<IEnumerable<Group>>(groups);
        }
        catch (Exception ex)
        {
            return new Response<IEnumerable<Group>>(
                ErrorCode.UnexpectedError,
                $"Error retrieving groups: {ex.Message}"
            );
        }
    }

    public async Task<Response<Group>> GetByIdAsync(Guid id, CancellationToken ctx = default)
    {
        try
        {
            var group = await dbContext
                .Groups.Where(g => g.Id == id)
                .Where(g => !g.IsDeleted)
                .Include(g => g.GroupPermissions.Where(gp => !gp.IsDeleted))
                .ThenInclude(gp => gp.Permission)
                .Include(g => g.Users.Where(u => !u.IsDeleted))
                .ThenInclude(u => u.User)
                .FirstOrDefaultAsync(ctx);

            return group == null
                ? new Response<Group>(ErrorCode.NotFound, $"Group with ID {id} not found")
                : new Response<Group>(group);
        }
        catch (Exception ex)
        {
            return new Response<Group>(
                ErrorCode.UnexpectedError,
                $"Error retrieving group: {ex.Message}"
            );
        }
    }

    public async Task<Response<bool>> AddUserToGroupAsync(
        Guid userId,
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        try
        {
            var membership = new UserGroupMembership
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                GroupId = groupId,
                CreatedAtDateTime = DateTime.UtcNow,
                IsDeleted = false,
            };

            await dbContext.UserGroupMemberships.AddAsync(membership, ctx);
            return new Response<bool>((await dbContext.SaveChangesAsync(ctx)) > 0);
        }
        catch (Exception ex)
        {
            return new Response<bool>(
                ErrorCode.UnexpectedError,
                $"Error adding user to group: {ex.Message}"
            );
        }
    }

    public async Task<Response<bool>> RemoveUserFromGroupAsync(
        Guid userId,
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        try
        {
            var membership = await dbContext.UserGroupMemberships.FirstOrDefaultAsync(
                m => m.UserId == userId && m.GroupId == groupId && !m.IsDeleted,
                ctx
            );

            if (membership == null)
                return new Response<bool>(
                    ErrorCode.NotFound,
                    "User is not a member of the specified group"
                );

            membership.IsDeleted = true;
            membership.EditedDateTime = DateTime.UtcNow;
            membership.Version++;
            membership.EditedById = ConstantIdValues.EditedById;

            dbContext.UserGroupMemberships.Update(membership);
            return new Response<bool>((await dbContext.SaveChangesAsync(ctx)) > 0);
        }
        catch (Exception ex)
        {
            return new Response<bool>(
                ErrorCode.UnexpectedError,
                $"Error removing user from group: {ex.Message}"
            );
        }
    }

    public async Task<Response<bool>> AddPermissionToGroupAsync(
        Guid permissionId,
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        try
        {
            var groupPermission = new GroupPermission
            {
                Id = Guid.NewGuid(),
                PermissionId = permissionId,
                GroupId = groupId,
                CreatedAtDateTime = DateTime.UtcNow,
                IsDeleted = false,
            };

            await dbContext.Set<GroupPermission>().AddAsync(groupPermission, ctx);
            return new Response<bool>((await dbContext.SaveChangesAsync(ctx)) > 0);
        }
        catch (Exception ex)
        {
            return new Response<bool>(
                ErrorCode.UnexpectedError,
                $"Error adding permission to group: {ex.Message}"
            );
        }
    }

    public async Task<Response<bool>> RemovePermissionFromGroupAsync(
        Guid permissionId,
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        try
        {
            var groupPermission = await dbContext
                .Set<GroupPermission>()
                .FirstOrDefaultAsync(
                    gp => gp.PermissionId == permissionId && gp.GroupId == groupId && !gp.IsDeleted,
                    ctx
                );

            if (groupPermission == null)
                return new Response<bool>(
                    ErrorCode.NotFound,
                    "Permission is not assigned to the specified group"
                );

            groupPermission.IsDeleted = true;
            groupPermission.EditedDateTime = DateTime.UtcNow;
            groupPermission.Version++;
            groupPermission.EditedById = ConstantIdValues.EditedById;

            dbContext.Set<GroupPermission>().Update(groupPermission);
            return new Response<bool>((await dbContext.SaveChangesAsync(ctx)) > 0);
        }
        catch (Exception ex)
        {
            return new Response<bool>(
                ErrorCode.UnexpectedError,
                $"Error removing permission from group: {ex.Message}"
            );
        }
    }

    public async Task<Response<bool>> AddUserPermissionInGroupAsync(
        Guid userId,
        Guid groupId,
        Guid permissionId,
        CancellationToken ctx = default
    )
    {
        try
        {
            var userPermission = new UserPermission
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                GroupId = groupId,
                PermissionId = permissionId,
                CreatedAtDateTime = DateTime.UtcNow,
                IsDeleted = false,
                Version = 1,
                EditedById = ConstantIdValues.EditedById,
            };

            await dbContext.UserPermissions.AddAsync(userPermission, ctx);
            return new Response<bool>((await dbContext.SaveChangesAsync(ctx)) > 0);
        }
        catch (Exception ex)
        {
            return new Response<bool>(
                ErrorCode.UnexpectedError,
                $"Error adding permission to user in group: {ex.Message}"
            );
        }
    }

    public async Task<Response<bool>> RemoveUserPermissionInGroupAsync(
        Guid userId,
        Guid groupId,
        Guid permissionId,
        CancellationToken ctx = default
    )
    {
        try
        {
            var userPermission = await dbContext.UserPermissions.FirstOrDefaultAsync(
                up =>
                    up.UserId == userId
                    && up.GroupId == groupId
                    && up.PermissionId == permissionId
                    && !up.IsDeleted,
                ctx
            );

            if (userPermission == null)
                return new Response<bool>(
                    ErrorCode.NotFound,
                    "Permission is not assigned to the user in this group"
                );

            userPermission.IsDeleted = true;
            userPermission.EditedDateTime = DateTime.UtcNow;
            userPermission.Version++;
            userPermission.EditedById = ConstantIdValues.EditedById;

            dbContext.UserPermissions.Update(userPermission);
            return new Response<bool>((await dbContext.SaveChangesAsync(ctx)) > 0);
        }
        catch (Exception ex)
        {
            return new Response<bool>(
                ErrorCode.UnexpectedError,
                $"Error removing permission from user in group: {ex.Message}"
            );
        }
    }
}

using Microsoft.EntityFrameworkCore;
using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;
using UserAccessSystem.Contract.Requests;
using UserAccessSystem.Domain.Permissions;
using UserAccessSystem.Domain.User;
using UserAccessSystem.Domain.UserDetail;
using UserAccessSystem.Internal.Application.Peristence;
using UserAccessSystem.Internal.Persistence.Common;
using UserAccessSystem.Internal.Persistence.DbContext;

namespace UserAccessSystem.Internal.Persistence.Repository;

public class UserRepository : Repository<User>, IUserRepository
{
    private UserAccessDbContext dbContext { get; }

    public UserRepository(UserAccessDbContext dbContext)
        : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Response<IEnumerable<User>>> GetAllAsync(
        DateTime? lastEntry,
        int size,
        CancellationToken ctx = default
    )
    {
        try
        {
            IQueryable<User> cursorQuery = dbContext
                .Users.Include(u => u.Groups.Where(g => !g.IsDeleted))
                .Include(u => u.UserPermissions.Where(p => !p.IsDeleted));

            if (lastEntry.HasValue)
            {
                cursorQuery = cursorQuery.Where(u => u.CreatedAtDateTime > lastEntry);
            }

            var users = await cursorQuery
                .Where(u => !u.IsDeleted)
                .OrderBy(u => u.CreatedAtDateTime)
                .ThenBy(u => u.Id)
                .Take(size)
                .ToListAsync(cancellationToken: ctx);

            return new Response<IEnumerable<User>>(users);
        }
        catch (Exception ex)
        {
            return new Response<IEnumerable<User>>(
                ErrorCode.UnexpectedError,
                $"Error retrieving users: {ex.Message}"
            );
        }
    }

    public async Task<Response<User>> GetByUsernameAsync(
        string username,
        CancellationToken ctx = default
    )
    {
        try
        {
            var response = await FindAsync(u => u.Username == username);
            if (!response.Success)
                return new Response<User>(response.ErrorCode, response.Message);

            var user = response.Data.FirstOrDefault();
            return user == null
                ? new Response<User>(
                    ErrorCode.UserNotFound,
                    $"User with username '{username}' not found"
                )
                : new Response<User>(user);
        }
        catch (Exception ex)
        {
            return new Response<User>(
                ErrorCode.UnexpectedError,
                $"Error retrieving user by username: {ex.Message}"
            );
        }
    }

    public async Task<Response<User>> GetAllUserPermissionsAsync(
        Guid userId,
        CancellationToken ctx = default
    )
    {
        try
        {
            var user = await dbContext
                .Users.Where(u => u.Id == userId)
                .Where(u => !u.IsDeleted)
                .Include(u => u.UserPermissions.Where(p => !p.IsDeleted))
                .ThenInclude(p => p.Permission)
                .FirstOrDefaultAsync(cancellationToken: ctx);

            if (user == null)
                return new Response<User>(
                    ErrorCode.UserNotFound,
                    $"User with ID {userId} not found"
                );

            return new Response<User>(user);
        }
        catch (Exception ex)
        {
            return new Response<User>(
                ErrorCode.UnexpectedError,
                $"Error retrieving user with permissions: {ex.Message}"
            );
        }
    }

    public async Task<Response<User>> GetUserGroupMembershipsAsync(
        Guid userId,
        CancellationToken ctx = default
    )
    {
        try
        {
            var user = await dbContext
                .Users.Where(u => u.Id == userId)
                .Where(u => !u.IsDeleted)
                .Include(u => u.Groups.Where(g => !g.IsDeleted))
                .ThenInclude(g => g.Group)
                .FirstOrDefaultAsync(cancellationToken: ctx);

            if (user == null)
                return new Response<User>(
                    ErrorCode.UserNotFound,
                    $"User with ID {userId} not found"
                );

            user.Groups = user.Groups.Where(g => g.Group != null && !g.Group.IsDeleted).ToList();

            return new Response<User>(user);
        }
        catch (Exception ex)
        {
            return new Response<User>(
                ErrorCode.UnexpectedError,
                $"Error retrieving user with group memberships: {ex.Message}"
            );
        }
    }

    public async Task<Response<User>> AddUserAsync(
        CreateUserRequest request,
        CancellationToken ctx = default
    )
    {
        try
        {
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                LockStatus = LockStatus.None,
                Password = request.Password,
            };

            return await base.AddAsync(newUser);
        }
        catch (Exception ex)
        {
            return new Response<User>(
                ErrorCode.UnexpectedError,
                $"Error creating user: {ex.Message}"
            );
        }
    }

    public async Task<Response<bool>> AddToGroupAsync(
        Guid id,
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        try
        {
            var existingMembership = await dbContext.UserGroupMemberships.FirstOrDefaultAsync(
                m => m.UserId == id && m.GroupId == groupId,
                ctx
            );

            if (existingMembership != null)
            {
                if (!existingMembership.IsDeleted)
                {
                    return new Response<bool>(
                        ErrorCode.UnexpectedError,
                        "User is already a member of this group"
                    );
                }

                existingMembership.IsDeleted = false;
                existingMembership.EditedDateTime = DateTime.UtcNow;
                existingMembership.Version++;
                existingMembership.EditedById = ConstantIdValues.EditedById;

                dbContext.UserGroupMemberships.Update(existingMembership);
            }
            else
            {
                var membership = new UserGroupMembership
                {
                    Id = Guid.NewGuid(),
                    UserId = id,
                    GroupId = groupId,
                    CreatedAtDateTime = DateTime.UtcNow,
                    IsDeleted = false,
                    Version = 1,
                    EditedById = ConstantIdValues.EditedById,
                };

                await dbContext.UserGroupMemberships.AddAsync(membership, ctx);
            }

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

    public async Task<Response<bool>> UpdateAsync(
        UpdateUserRequest request,
        CancellationToken ctx = default
    )
    {
        try
        {
            var userResponse = await GetByUsernameAsync(request.Username, ctx);
            if (!userResponse.Success)
                return new Response<bool>(userResponse.ErrorCode, userResponse.Message);

            var user = userResponse.Data;
            user.Email = request.Email;
            user.Username = request.Username;

            return await base.UpdateAsync(user);
        }
        catch (Exception ex)
        {
            return new Response<bool>(
                ErrorCode.UnexpectedError,
                $"Error updating user: {ex.Message}"
            );
        }
    }

    public async Task<Response<bool>> DeleteAsync(Guid id, CancellationToken ctx = default)
    {
        var getResponse = await GetByIdAsync(id, ctx);
        if (!getResponse.Success)
            return new Response<bool>(getResponse.ErrorCode, getResponse.Message);

        return await DeleteAsync(getResponse.Data);
    }

    public async Task<Response<bool>> RemoveFromGroupAsync(
        Guid id,
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        try
        {
            var membership = await dbContext.UserGroupMemberships.FirstOrDefaultAsync(
                m => m.UserId == id && m.GroupId == groupId && !m.IsDeleted,
                ctx
            );

            if (membership == null)
            {
                return new Response<bool>(ErrorCode.NotFound, "User is not a member of this group");
            }

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

    public async Task<Response<IEnumerable<Permission>>> GetUserPermissionsAsync(
        Guid userId,
        CancellationToken ctx = default
    )
    {
        try
        {
            var user = await dbContext
                .Users.Where(u => u.Id == userId && !u.IsDeleted)
                .Include(u => u.UserPermissions.Where(up => !up.IsDeleted))
                .ThenInclude(up => up.Permission)
                .Include(u => u.UserPermissions.Where(up => !up.IsDeleted))
                .ThenInclude(up => up.Group)
                .Include(u => u.Groups.Where(g => !g.IsDeleted))
                .ThenInclude(g => g.Group)
                .ThenInclude(g => g.GroupPermissions.Where(gp => !gp.IsDeleted))
                .ThenInclude(gp => gp.Permission)
                .FirstOrDefaultAsync(ctx);

            if (user == null)
                return new Response<IEnumerable<Permission>>(ErrorCode.NotFound, "User not found");

            var directPermissions = user.UserPermissions.Select(up =>
            {
                up.Permission.SourceGroupId = up.GroupId;
                up.Permission.SourceType = (int)PermissionSourceType.UserInGroup;
                return up.Permission;
            });

            var groupPermissions = user.Groups.SelectMany(g =>
                g.Group.GroupPermissions.Select(gp =>
                {
                    gp.Permission.SourceGroupId = g.GroupId;
                    gp.Permission.SourceType = (int)PermissionSourceType.GroupLevel;
                    return gp.Permission;
                })
            );

            var allPermissions = directPermissions.Union(groupPermissions);
            return new Response<IEnumerable<Permission>>(allPermissions);
        }
        catch (Exception ex)
        {
            return new Response<IEnumerable<Permission>>(
                ErrorCode.UnexpectedError,
                $"Error retrieving user permissions: {ex.Message}"
            );
        }
    }

    public async Task<Response<bool>> AddPermissionToUserAsync(
        Guid userId,
        Guid permissionId,
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        try
        {
            var existingPermission = await dbContext.UserPermissions.FirstOrDefaultAsync(
                up =>
                    up.UserId == userId && up.GroupId == groupId && up.PermissionId == permissionId,
                ctx
            );

            if (existingPermission != null)
            {
                if (!existingPermission.IsDeleted)
                {
                    return new Response<bool>(
                        ErrorCode.UnexpectedError,
                        "Permission is already assigned to the user in this group"
                    );
                }

                existingPermission.IsDeleted = false;
                existingPermission.EditedDateTime = DateTime.UtcNow;
                existingPermission.Version++;
                existingPermission.EditedById = ConstantIdValues.EditedById;

                dbContext.UserPermissions.Update(existingPermission);
            }
            else
            {
                var userPermission = new UserPermission
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    PermissionId = permissionId,
                    GroupId = groupId,
                    CreatedAtDateTime = DateTime.UtcNow,
                    IsDeleted = false,
                    Version = 1,
                    EditedById = ConstantIdValues.EditedById,
                };

                await dbContext.UserPermissions.AddAsync(userPermission, ctx);
            }

            return new Response<bool>((await dbContext.SaveChangesAsync(ctx)) > 0);
        }
        catch (Exception ex)
        {
            return new Response<bool>(
                ErrorCode.UnexpectedError,
                $"Error adding permission to user: {ex.Message}"
            );
        }
    }

    public async Task<Response<bool>> RemovePermissionFromUserAsync(
        Guid userId,
        Guid permissionId,
        Guid groupId,
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
            {
                return new Response<bool>(
                    ErrorCode.NotFound,
                    "Permission is not assigned to the user in this group"
                );
            }

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
                $"Error removing permission from user: {ex.Message}"
            );
        }
    }
}

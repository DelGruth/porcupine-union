using Microsoft.EntityFrameworkCore;
using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Requests;
using UserAccessSystem.Domain.Permissions;
using UserAccessSystem.Domain.User;
using UserAccessSystem.Domain.UserDetail;
using UserAccessSystem.Internal.Application.Peristence;
using UserAccessSystem.Internal.Persistence.Common;
using UserAccessSystem.Internal.Persistence.DbContext;

namespace UserAccessSystem.Internal.Persistence.Repository;

public class UserRepository : Repository<User?>, IUserRepository
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
                .ThenInclude(g => g.GroupPermissions.Where(gp => !gp.IsDeleted))
                .ThenInclude(gp => gp.Permission)
                .Include(u => u.UserPermissions.Where(p => !p.IsDeleted))
                .ThenInclude(p => p.Permission)
                .FirstOrDefaultAsync(ctx);

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
                $"Error retrieving user with groups and permissions: {ex.Message}"
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
                Email = request.Username,
                LockStatus = LockStatus.None,
                Password = "AllPasswordsAreUnique",
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
            dbContext.UserGroupMemberships.Add(
                new UserGroupMembership() { UserId = id, GroupId = groupId }
            );
            return new Response<bool>((await dbContext.SaveChangesAsync(ctx)) > 0);
        }
        catch (Exception ex)
        {
            return new Response<bool>(
                ErrorCode.UnexpectedError,
                $"Error creating user: {ex.Message}"
            );
        }
    }

    public async Task<Response<bool>> UpdateAsync(
        CreateUserRequest request,
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
        var getResponse = await GetByIdAsync(id);
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
            var groupMembership = await dbContext.UserGroupMemberships.FirstOrDefaultAsync(
                ug => ug.UserId == id && ug.GroupId == groupId,
                cancellationToken: ctx
            );

            groupMembership!.IsDeleted = true;
            dbContext.Entry(groupMembership).State = EntityState.Modified;
            return new Response<bool>((await dbContext.SaveChangesAsync(ctx)) > 0);
        }
        catch (Exception ex)
        {
            return new Response<bool>(
                ErrorCode.UnexpectedError,
                $"Error deleting user: {ex.Message}"
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
                .Users.Where(u => u.Id == userId)
                .Where(u => !u.IsDeleted)
                .Include(u => u.UserPermissions.Where(p => !p.IsDeleted))
                .ThenInclude(p => p.Permission)
                .Include(u => u.Groups.Where(g => !g.IsDeleted))
                .ThenInclude(g => g.Group)
                .ThenInclude(g => g.GroupPermissions.Where(gp => !gp.IsDeleted))
                .ThenInclude(gp => gp.Permission)
                .FirstOrDefaultAsync(ctx);

            if (user == null)
                return new Response<IEnumerable<Permission>>(
                    ErrorCode.UserNotFound,
                    $"User with ID {userId} not found"
                );

            // Combine direct user permissions and group permissions
            var permissions = user
                .UserPermissions.Select(up => up.Permission)
                .Union(
                    user.Groups.SelectMany(g => g.Group.GroupPermissions)
                        .Select(gp => gp.Permission)
                )
                .Distinct();

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

    public async Task<Response<bool>> AddPermissionToUserAsync(
        Guid userId,
        Guid permissionId,
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        try
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
                $"Error removing permission from user: {ex.Message}"
            );
        }
    }
}

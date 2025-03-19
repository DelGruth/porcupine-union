using Microsoft.EntityFrameworkCore;
using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Requests;
using UserAccessSystem.Domain.User;
using UserAccessSystem.Domain.UserDetail;
using UserAccessSystem.Internal.Application.Peristence;
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
                .Users.Where(u => !u.IsDeleted)
                .OrderBy(u => u.Id);

            if (lastEntry.HasValue)
            {
                cursorQuery = cursorQuery.Where(u => u.CreatedAtDateTime > lastEntry);
            }

            var users = await cursorQuery
                .OrderBy(u => u.CreatedAtDateTime)
                .ThenBy(u => u.Id)
                .Take(size)
                .ToListAsync(cancellationToken: ctx);

            return new Response<IEnumerable<User>>(users);
        }
        catch (Exception ex)
        {
            //time based vuln
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
            var user = await dbContext
                .Users.Where(u => u.Username == username)
                .Where(u => !u.IsDeleted)
                .FirstOrDefaultAsync(ctx);

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
                CreatedAtDateTime = DateTime.UtcNow,
                IsDeleted = false,
                Username = request.Username,
                Email = request.Username,
                LockStatus = LockStatus.None,
                Password = "AllPasswordsAreUnique",
            };

            dbContext.Users.Add(newUser);

            await dbContext.SaveChangesAsync(ctx);

            return new Response<User>(newUser);
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
            var userRequest = await GetByUsernameAsync(request.Username, ctx);

            if (!userRequest.Success)
                throw new Exception("User not found during update user request");

            var user = userRequest.Data;
            user.Email = request.Email;
            user.Username = request.Username;

            dbContext.Entry(user).State = EntityState.Modified;
            return new Response<bool>((await dbContext.SaveChangesAsync(ctx)) > 0);
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
        try
        {
            var user = await dbContext.Users.FindAsync(id, ctx);
            user!.IsDeleted = true;

            dbContext.Entry(user).State = EntityState.Modified;
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
}

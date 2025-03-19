using Microsoft.EntityFrameworkCore;
using UserAccessSystem.Contract;
using UserAccessSystem.Domain.Permissions;
using UserAccessSystem.Domain.User;
using UserAccessSystem.Internal.Application.Peristence;
using UserAccessSystem.Internal.Persistence.DbContext;

namespace UserAccessSystem.Internal.Persistence.Repository;

public class PermissionRepository(UserAccessDbContext dbContext)
    : Repository<Permission>(dbContext),
        IPermissionRepository
{
    public async Task<Response<IEnumerable<Permission>>> GetAllAsync(
        CancellationToken ctx = default
    )
    {
        try
        {
            return await base.GetAllAsync();
        }
        catch (Exception ex)
        {
            return new Response<IEnumerable<Permission>>(
                ErrorCode.UnexpectedError,
                $"Error retrieving permissions: {ex.Message}"
            );
        }
    }
}

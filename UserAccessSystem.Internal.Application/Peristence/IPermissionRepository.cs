using UserAccessSystem.Contract;
using UserAccessSystem.Domain.Permissions;

namespace UserAccessSystem.Internal.Application.Peristence;

public interface IPermissionRepository : IRepository<Permission>
{
    Task<Response<IEnumerable<Permission>>> GetAllAsync(CancellationToken ctx = default);
    Task<Response<IEnumerable<Permission>>> GetPermissionsByGroupIdAsync(
        Guid groupId,
        CancellationToken ctx = default
    );
    Task<Response<IEnumerable<Permission>>> GetPermissionsByUserIdAsync(
        Guid userId,
        CancellationToken ctx = default
    );
    Task<Response<Permission>> CreateAsync(Permission permission, CancellationToken ctx = default);
}

using UserAccessSystem.Contract;
using UserAccessSystem.Domain.Permissions;

namespace UserAccessSystem.Internal.Application.Peristence;

public interface IPermissionRepository
{
    Task<Response<IEnumerable<Permission>>> GetAllAsync(CancellationToken ctx = default);
}

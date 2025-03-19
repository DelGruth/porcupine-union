using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;
using UserAccessSystem.Contract.Requests;

namespace UserAccessSystem.Internal.Application.Infrastructure;

public interface IPermissionService
{
    ValueTask<Response<IEnumerable<PermissionDto>>> GetAllPermissionsAsync(
        CancellationToken ctx = default
    );
    Task<Response<UserDto>> CreateAsync(
        CreatePermissionRequest request,
        CancellationToken ctx = default
    );
    Task<Response<bool>> DeleteAsync(Guid id, CancellationToken ctx = default);
}

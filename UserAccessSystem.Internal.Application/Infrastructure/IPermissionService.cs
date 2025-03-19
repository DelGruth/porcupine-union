using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;

namespace UserAccessSystem.Internal.Application.Infrastructure;

public interface IPermissionService
{
    Task<Response<PermissionDto>> GetAllPermissions();
}

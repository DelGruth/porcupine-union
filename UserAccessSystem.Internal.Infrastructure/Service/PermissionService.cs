using Microsoft.Extensions.Caching.Hybrid;
using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;
using UserAccessSystem.Internal.Application.Infrastructure;

namespace UserAccessSystem.Internal.Infrastructure.Service;

public class PermissionService(HybridCache cache) : IPermissionService
{
    public Task<Response<PermissionDto>> GetAllPermissions()
    {
        throw new NotImplementedException();
    }
}

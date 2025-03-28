using Microsoft.Extensions.Caching.Hybrid;
using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;
using UserAccessSystem.Contract.Requests;
using UserAccessSystem.Domain.Permissions;
using UserAccessSystem.Internal.Application.Infrastructure;
using UserAccessSystem.Internal.Application.Peristence;

namespace UserAccessSystem.Internal.Infrastructure.Service;

public class PermissionService(HybridCache cache, IPermissionRepository permissionRepository)
    : BaseService<Permission, PermissionDto>(cache, permissionRepository),
        IPermissionService
{
    protected override PermissionDto MapToDto(Permission entity) => new(entity);

    public async ValueTask<Response<IEnumerable<PermissionDto>>> GetAllPermissionsAsync(
        CancellationToken ctx = default
    ) => await GetAllAsync(ctx);

    public async Task<Response<PermissionDto>> CreateAsync(
        CreatePermissionRequest request,
        CancellationToken ctx = default
    )
    {
        var permission = new Permission
        {
            Name = request.Name,
            Description = request.Description,
            ReadOnly = request.ReadOnly,
            WriteOnly = request.WriteOnly,
        };

        var result = await permissionRepository.AddAsync(permission, ctx);
        return !result.Success
            ? new Response<PermissionDto>(result.ErrorCode, result.Message)
            : new Response<PermissionDto>(MapToDto(result.Data));
    }

    public override async Task<Response<bool>> DeleteAsync(Guid id, CancellationToken ctx = default)
    {
        var result = await base.DeleteAsync(id, ctx);
        if (result.Success)
        {
            await InvalidatePermissionCache(id);
        }
        return result;
    }

    private async Task InvalidatePermissionCache(Guid id)
    {
        var cacheKey = $"GetById_Permission_{id}";
        await cache.RemoveAsync(cacheKey);
    }
}

using Microsoft.Extensions.Caching.Hybrid;
using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;
using UserAccessSystem.Contract.Requests;
using UserAccessSystem.Internal.Application.Infrastructure;
using UserAccessSystem.Internal.Application.Peristence;

namespace UserAccessSystem.Internal.Infrastructure.Service;

public class PermissionService(HybridCache cache, IPermissionRepository permissionRepository)
    : IPermissionService
{
    public async ValueTask<Response<IEnumerable<PermissionDto>>> GetAllPermissionsAsync(
        CancellationToken ctx = default
    )
    {
        const string cacheKey = $"GetAllPermissions";
        return await cache.GetOrCreateAsync<Response<IEnumerable<PermissionDto>>>(
            cacheKey,
            async ctx =>
            {
                var dataRequest = await permissionRepository.GetAllAsync(ctx);

                if (!dataRequest.Success)
                    return new Response<IEnumerable<PermissionDto>>(ErrorCode.UnexpectedError);

                return new Response<IEnumerable<PermissionDto>>(
                    dataRequest.Data?.Select(x => new PermissionDto(x)) ?? []
                );
            },
            options: new HybridCacheEntryOptions() { Expiration = TimeSpan.FromMilliseconds(150) },
            cancellationToken: ctx
        );
    }

    public async Task<Response<UserDto>> CreateAsync(
        CreatePermissionRequest request,
        CancellationToken ctx = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<Response<bool>> DeleteAsync(Guid id, CancellationToken ctx = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<bool>> RemoveFromGroup(
        Guid id,
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        throw new NotImplementedException();
    }
}

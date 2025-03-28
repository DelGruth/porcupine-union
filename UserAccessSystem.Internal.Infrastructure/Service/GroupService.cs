using Microsoft.Extensions.Caching.Hybrid;
using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;
using UserAccessSystem.Internal.Application.Infrastructure;
using UserAccessSystem.Internal.Application.Peristence;

namespace UserAccessSystem.Internal.Infrastructure.Service;

public class GroupService(HybridCache cache, IGroupRepository groupRepository) : IGroupService
{
    public async Task<Response<IEnumerable<GroupDto>>> GetAllGroupsAsync(
        CancellationToken ctx = default
    )
    {
        const string cacheKey = "GetAllGroups";
        return await cache.GetOrCreateAsync<Response<IEnumerable<GroupDto>>>(
            cacheKey,
            async ctx =>
            {
                var dataRequest = await groupRepository.GetAllAsync(ctx);

                if (!dataRequest.Success)
                    return new Response<IEnumerable<GroupDto>>(ErrorCode.UnexpectedError);

                return new Response<IEnumerable<GroupDto>>(
                    dataRequest.Data?.Select(x => new GroupDto(x)) ?? []
                );
            },
            options: new HybridCacheEntryOptions() { Expiration = TimeSpan.FromMilliseconds(150) },
            cancellationToken: ctx
        );
    }

    public async Task<Response<GroupDto>> GetByIdAsync(Guid id, CancellationToken ctx = default)
    {
        var result = await groupRepository.GetByIdAsync(id, ctx);
        return !result.Success
            ? new Response<GroupDto>(result.ErrorCode)
            : new Response<GroupDto>(new GroupDto(result.Data));
    }

    public async Task<Response<bool>> AddUserToGroupAsync(
        Guid userId,
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        var result = await groupRepository.AddUserToGroupAsync(userId, groupId, ctx);
        if (result.Success)
        {
            await InvalidateGroupCache(groupId);
        }
        return result;
    }

    public async Task<Response<bool>> RemoveUserFromGroupAsync(
        Guid userId,
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        var result = await groupRepository.RemoveUserFromGroupAsync(userId, groupId, ctx);
        if (result.Success)
        {
            await InvalidateGroupCache(groupId);
        }
        return result;
    }

    public async Task<Response<bool>> AddPermissionToGroupAsync(
        Guid permissionId,
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        var result = await groupRepository.AddPermissionToGroupAsync(permissionId, groupId, ctx);
        if (result.Success)
        {
            await InvalidateGroupCache(groupId);
        }
        return result;
    }

    public async ValueTask<Response<bool>> RemovePermissionFromGroupAsync(
        Guid permissionId,
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        var result = await groupRepository.RemovePermissionFromGroupAsync(
            permissionId,
            groupId,
            ctx
        );
        if (result.Success)
        {
            await InvalidateGroupCache(groupId);
        }
        return result;
    }

    private ValueTask InvalidateGroupCache(Guid groupId)
    {
        const string allGroupsCacheKey = "GetAllGroups";
        var groupCacheKey = $"Group_{groupId}";

        _ = cache.RemoveAsync(allGroupsCacheKey);
        _ = cache.RemoveAsync(groupCacheKey);
        return ValueTask.CompletedTask;
    }
}

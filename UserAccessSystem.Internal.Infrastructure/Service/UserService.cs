using Microsoft.Extensions.Caching.Hybrid;
using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;
using UserAccessSystem.Contract.Requests;
using UserAccessSystem.Domain.User;
using UserAccessSystem.Domain.UserDetail;
using UserAccessSystem.Internal.Application.Infrastructure;
using UserAccessSystem.Internal.Application.Peristence;

namespace UserAccessSystem.Internal.Infrastructure.Service;

public class UserService(HybridCache cache, IUserRepository userRepository)
    : BaseService<User, UserDto>(cache, userRepository),
        IUserService
{
    protected override UserDto MapToDto(User entity) => new(entity);

    public async ValueTask<Response<IEnumerable<UserDto>>> GetAllUsersAsync(
        DateTime? lastEntry,
        CancellationToken ctx = default
    )
    {
        const string cacheKey = "GetAllUsers";
        return await cache.GetOrCreateAsync<Response<IEnumerable<UserDto>>>(
            cacheKey,
            async ctx =>
            {
                var dataRequest = await userRepository.GetAllAsync(lastEntry, 100, ctx);
                return !dataRequest.Success
                    ? new Response<IEnumerable<UserDto>>(ErrorCode.UnexpectedError)
                    : new Response<IEnumerable<UserDto>>(dataRequest.Data?.Select(MapToDto) ?? []);
            },
            options: new HybridCacheEntryOptions() { Expiration = TimeSpan.FromMilliseconds(150) },
            cancellationToken: ctx
        );
    }

    public async Task<Response<UserDto>> CreateAsync(
        CreateUserRequest request,
        CancellationToken ctx = default
    )
    {
        var result = await userRepository.AddUserAsync(request, ctx);
        return !result.Success
            ? new Response<UserDto>(result.ErrorCode)
            : new Response<UserDto>(MapToDto(result.Data));
    }

    public async Task<Response<bool>> UpdateAsync(
        CreateUserRequest request,
        CancellationToken ctx = default
    ) => await userRepository.UpdateAsync(request, ctx);

    public async Task<Response<bool>> AddUserToGroupAsync(
        Guid userId,
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        var result = await userRepository.AddToGroupAsync(userId, groupId, ctx);
        if (result.Success)
        {
            await InvalidateUserCache(userId);
        }
        return result;
    }

    public async Task<Response<bool>> RemoveUserFromGroupAsync(
        Guid userId,
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        var result = await userRepository.RemoveFromGroupAsync(userId, groupId, ctx);
        if (result.Success)
        {
            await InvalidateUserCache(userId);
        }
        return result;
    }

    public async Task<Response<IEnumerable<PermissionDto>>> GetUserPermissionsAsync(
        Guid userId,
        CancellationToken ctx = default
    )
    {
        var result = await userRepository.GetUserPermissionsAsync(userId, ctx);
        return !result.Success
            ? new Response<IEnumerable<PermissionDto>>(result.ErrorCode, result.Message)
            : new Response<IEnumerable<PermissionDto>>(
                result.Data.Select(p => new PermissionDto(p))
            );
    }

    public async Task<Response<bool>> AddPermissionToUserAsync(
        Guid userId,
        Guid permissionId,
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        var result = await userRepository.AddPermissionToUserAsync(
            userId,
            permissionId,
            groupId,
            ctx
        );
        if (result.Success)
        {
            await InvalidateUserCache(userId);
        }
        return result;
    }

    public async Task<Response<bool>> RemovePermissionFromUserAsync(
        Guid userId,
        Guid permissionId,
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        var result = await userRepository.RemovePermissionFromUserAsync(
            userId,
            permissionId,
            groupId,
            ctx
        );
        if (result.Success)
        {
            await InvalidateUserCache(userId);
        }
        return result;
    }

    private async Task InvalidateUserCache(Guid userId)
    {
        const string allUsersCacheKey = "GetAllUsers";
        var userCacheKey = $"GetById_User_{userId}";
        await cache.RemoveAsync(allUsersCacheKey);
        await cache.RemoveAsync(userCacheKey);
    }
}

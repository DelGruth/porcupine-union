using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;
using UserAccessSystem.Internal.Application.Infrastructure;
using UserAccessSystem.Internal.Application.Peristence;

namespace UserAccessSystem.Internal.Infrastructure.Service;

public class UserService(HybridCache cache, IUserRepository userRepository) : IUserService
{
    public async ValueTask<Response<IEnumerable<UserDto>>> GetAllUsers(
        DateTime? lastEntry,
        CancellationToken ctx = default
    )
    {
        const string cacheKey = $"GetAllUsers";
        return await cache.GetOrCreateAsync<Response<IEnumerable<UserDto>>>(
            cacheKey,
            async ctx =>
            {
                var dataRequest = await userRepository.GetAllAsync(lastEntry, 100);

                if (!dataRequest.Success)
                    return new Response<IEnumerable<UserDto>>(ErrorCode.UnexpectedError);

                return new Response<IEnumerable<UserDto>>(
                    dataRequest.Data?.Select(x => new UserDto(x)) ?? []
                );
            },
            options: new HybridCacheEntryOptions() { Expiration = TimeSpan.FromMilliseconds(150) },
            cancellationToken: ctx
        );
    }
}

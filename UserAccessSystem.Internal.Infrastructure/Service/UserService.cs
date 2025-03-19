using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;
using UserAccessSystem.Internal.Application.Infrastructure;

namespace UserAccessSystem.Internal.Infrastructure.Service;

public class UserService(HybridCache cache) : IUserService
{
    public Task<Response<UserDto>> GetAllUsers()
    {
        var cacheKey = $"GetAllUsers";
        cache.GetOrCreateAsync(cacheKey, async () => { });
    }
}

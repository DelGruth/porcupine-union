using UserAccessSystem.Contract;
using UserAccessSystem.Domain.User;

namespace UserAccessSystem.Internal.Application.Peristence;

public interface IUserRepository
{
    Task<Response<IEnumerable<User>>> GetAllAsync(
        DateTime? lastEntry,
        int size,
        CancellationToken ctx = default
    );
    Task<Response<User>> GetByUsernameAsync(string username, CancellationToken ctx = default);
    Task<Response<User>> GetAllUserPermissions(Guid userId, CancellationToken ctx = default);
    Task<Response<User>> GetUserWithGroupsAndPermissionsAsync(
        Guid userId,
        CancellationToken ctx = default
    );
}

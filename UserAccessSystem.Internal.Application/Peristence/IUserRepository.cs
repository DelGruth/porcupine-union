using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Requests;
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
    Task<Response<User>> GetAllUserPermissionsAsync(Guid userId, CancellationToken ctx = default);
    Task<Response<User>> GetUserGroupMembershipsAsync(Guid userId, CancellationToken ctx = default);
    Task<Response<User>> AddUserAsync(CreateUserRequest request, CancellationToken ctx = default);
    Task<Response<bool>> AddToGroupAsync(Guid id, Guid groupId, CancellationToken ctx = default);
    Task<Response<bool>> UpdateAsync(CreateUserRequest request, CancellationToken ctx = default);
    Task<Response<bool>> DeleteAsync(Guid id, CancellationToken ctx = default);
    Task<Response<bool>> RemoveFromGroupAsync(
        Guid id,
        Guid groupId,
        CancellationToken ctx = default
    );
}

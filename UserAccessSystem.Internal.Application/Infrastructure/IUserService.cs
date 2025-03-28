using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;
using UserAccessSystem.Contract.Requests;

namespace UserAccessSystem.Internal.Application.Infrastructure;

public interface IUserService
{
    ValueTask<Response<IEnumerable<UserDto>>> GetAllUsersAsync(
        DateTime? lastEntry,
        CancellationToken ctx = default
    );
    Task<Response<UserDto>> CreateAsync(CreateUserRequest request, CancellationToken ctx = default);
    Task<Response<bool>> UpdateAsync(CreateUserRequest request, CancellationToken ctx = default);
    Task<Response<bool>> DeleteAsync(Guid id, CancellationToken ctx = default);
    Task<Response<bool>> AddUserToGroupAsync(
        Guid userId,
        Guid groupId,
        CancellationToken ctx = default
    );
    Task<Response<bool>> RemoveUserFromGroupAsync(
        Guid userId,
        Guid groupId,
        CancellationToken ctx = default
    );
}

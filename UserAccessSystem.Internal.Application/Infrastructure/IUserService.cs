using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;
using UserAccessSystem.Contract.Requests;

namespace UserAccessSystem.Internal.Application.Infrastructure;

public interface IUserService
{
    ValueTask<Response<IEnumerable<UserDto>>> GetAllUsers(
        DateTime? lastEntry,
        CancellationToken ctx = default
    );

    Task<Response<UserDto>> Create(CreateUserRequest request, CancellationToken ctx = default);
    Task<Response<bool>> AddToGroup(Guid id, Guid groupId, CancellationToken ctx = default);
    Task<Response<bool>> Update(CreateUserRequest request, CancellationToken ctx = default);
    Task<Response<bool>> Delete(Guid id, CancellationToken ctx = default);
    Task<Response<bool>> RemoveFromGroup(Guid id, Guid groupId, CancellationToken ctx = default);
}

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

    Task<Response<bool>> Create(CreateUserRequest request);
    Task<Response<bool>> AddToGroup(Guid id, Guid groupId);
    Task<Response<bool>> Update(CreateUserRequest request);
    Task<Response<bool>> Delete(Guid id);
    Task<Response<bool>> RemoveFromGroup(Guid id, Guid groupId);
}

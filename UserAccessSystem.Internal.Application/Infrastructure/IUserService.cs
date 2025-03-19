using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;

namespace UserAccessSystem.Internal.Application.Infrastructure;

public interface IUserService
{
    ValueTask<Response<IEnumerable<UserDto>>> GetAllUsers(
        DateTime? lastEntry,
        CancellationToken ctx = default
    );
}

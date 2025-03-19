using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;

namespace UserAccessSystem.Internal.Application.Infrastructure;

public interface IUserService
{
    Task<Response<UserDto>> GetAllUsers();
}

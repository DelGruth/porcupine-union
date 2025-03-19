using UserAccessSystem.Contract.Dtos;

namespace UserAccessSystem.Contract.Responses;

public readonly struct GetAllUsersResponse
{
    public IEnumerable<UserDto> Users { get; }
}

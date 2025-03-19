using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;

namespace UserAccessSystem.Internal.Application.Infrastructure;

public interface IGroupService
{
    Task<Response<GroupDto>> GetAllGroupsAsync();
}

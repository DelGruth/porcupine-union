using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;
using UserAccessSystem.Internal.Application.Infrastructure;

namespace UserAccessSystem.Internal.Infrastructure.Service;

public class GroupService : IGroupService
{
    public Task<Response<GroupDto>> GetAllGroupsAsync()
    {
        throw new NotImplementedException();
    }
}

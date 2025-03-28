using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;

namespace UserAccessSystem.Internal.Application.Infrastructure;

public interface IGroupService
{
    Task<Response<IEnumerable<GroupDto>>> GetAllGroupsAsync(CancellationToken ctx = default);
    Task<Response<GroupDto>> GetByIdAsync(Guid id, CancellationToken ctx = default);
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
    Task<Response<bool>> AddPermissionToGroupAsync(
        Guid permissionId,
        Guid groupId,
        CancellationToken ctx = default
    );
    Task<Response<bool>> RemovePermissionFromGroupAsync(
        Guid permissionId,
        Guid groupId,
        CancellationToken ctx = default
    );
}

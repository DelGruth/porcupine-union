using UserAccessSystem.Contract;
using UserAccessSystem.Domain.Group;

namespace UserAccessSystem.Internal.Application.Peristence;

public interface IGroupRepository : IRepository<Group>
{
    Task<Response<IEnumerable<Group>>> GetAllAsync(CancellationToken ctx = default);
    Task<Response<Group>> GetByIdAsync(Guid id, CancellationToken ctx = default);
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

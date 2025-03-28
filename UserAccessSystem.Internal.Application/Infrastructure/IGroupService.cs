using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;
using UserAccessSystem.Contract.Requests;
using UserAccessSystem.Contract.Responses;

namespace UserAccessSystem.Internal.Application.Infrastructure;

public interface IGroupService
{
    Task<Response<GroupListResponse>> GetAllGroupsAsync(CancellationToken ctx = default);
    Task<Response<GroupDto>> GetByIdAsync(Guid id, CancellationToken ctx = default);
    Task<Response<GroupResponse>> GetGroupByIdAsync(Guid id, CancellationToken ctx = default);
    Task<Response<GroupResponse>> CreateAsync(
        CreateGroupRequest request,
        CancellationToken ctx = default
    );
    Task<Response<bool>> UpdateAsync(UpdateGroupRequest request, CancellationToken ctx = default);
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
    Task<Response<GroupUserDistributionResponse>> GetUsersPerGroupCountAsync(
        CancellationToken ctx = default
    );
    Task<Response<bool>> AddUserPermissionInGroupAsync(
        Guid userId,
        Guid groupId,
        Guid permissionId,
        CancellationToken ctx = default
    );
    Task<Response<bool>> RemoveUserPermissionInGroupAsync(
        Guid userId,
        Guid groupId,
        Guid permissionId,
        CancellationToken ctx = default
    );
}

using Microsoft.AspNetCore.Mvc;
using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Requests;
using UserAccessSystem.Contract.Responses;
using UserAccessSystem.Internal.Application.Infrastructure;
using UserAccessSystem.Internal.Infrastructure.Middleware;

namespace UserAccessSystem.Internal.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Auth("SU", true, true)]
public class GroupsController(IGroupService groupService) : ControllerBase
{
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<Response<GroupResponse>> Get([FromRoute] Guid id) =>
        await groupService.GetGroupByIdAsync(id);

    [HttpGet]
    public async Task<Response<GroupListResponse>> GetAll() =>
        await groupService.GetAllGroupsAsync();

    [HttpPost]
    public async Task<Response<GroupResponse>> Create([FromBody] CreateGroupRequest request) =>
        await groupService.CreateAsync(request);

    [HttpPut]
    public async Task<Response<bool>> Update([FromBody] UpdateGroupRequest request) =>
        await groupService.UpdateAsync(request);

    [HttpDelete("{id:guid}")]
    public async Task<Response<bool>> Delete([FromRoute] Guid id) =>
        await groupService.DeleteAsync(id);

    [HttpPost]
    [Route("{groupId:guid}/users/{userId:guid}")]
    public async Task<Response<bool>> AddUser([FromRoute] Guid groupId, [FromRoute] Guid userId) =>
        await groupService.AddUserToGroupAsync(userId, groupId);

    [HttpDelete]
    [Route("{groupId:guid}/users/{userId:guid}")]
    public async Task<Response<bool>> RemoveUser(
        [FromRoute] Guid groupId,
        [FromRoute] Guid userId
    ) => await groupService.RemoveUserFromGroupAsync(userId, groupId);

    [HttpPost]
    [Route("{groupId:guid}/permissions/{permissionId:guid}")]
    public async Task<Response<bool>> AddPermission(
        [FromRoute] Guid groupId,
        [FromRoute] Guid permissionId
    ) => await groupService.AddPermissionToGroupAsync(permissionId, groupId);

    [HttpDelete]
    [Route("{groupId:guid}/permissions/{permissionId:guid}")]
    public async Task<Response<bool>> RemovePermission(
        [FromRoute] Guid groupId,
        [FromRoute] Guid permissionId
    ) => await groupService.RemovePermissionFromGroupAsync(permissionId, groupId);

    [HttpGet]
    [Route("users/distribution")]
    public async Task<Response<GroupUserDistributionResponse>> GetUsersPerGroupCount() =>
        await groupService.GetUsersPerGroupCountAsync();

    [HttpPost]
    [Route("{groupId:guid}/users/{userId:guid}/permissions/{permissionId:guid}")]
    public async Task<Response<bool>> AddUserPermissionInGroup(
        [FromRoute] Guid groupId,
        [FromRoute] Guid userId,
        [FromRoute] Guid permissionId
    ) => await groupService.AddUserPermissionInGroupAsync(userId, groupId, permissionId);

    [HttpDelete]
    [Route("{groupId:guid}/users/{userId:guid}/permissions/{permissionId:guid}")]
    public async Task<Response<bool>> RemoveUserPermissionInGroup(
        [FromRoute] Guid groupId,
        [FromRoute] Guid userId,
        [FromRoute] Guid permissionId
    ) => await groupService.RemoveUserPermissionInGroupAsync(userId, groupId, permissionId);
}

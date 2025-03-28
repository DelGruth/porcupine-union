using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;
using UserAccessSystem.Contract.Requests;
using UserAccessSystem.Internal.Application.Infrastructure;
using UserAccessSystem.Internal.Infrastructure.Middleware;

namespace UserAccessSystem.Internal.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Auth("SU", true, true)]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    [Auth("SU", true, false)]
    public async ValueTask<Response<IEnumerable<UserDto>>> GetAll(
        [FromQuery] DateTime? lastEntry,
        CancellationToken ctx = default
    ) => await userService.GetAllUsersAsync(lastEntry, ctx);

    [HttpPost]
    public async Task<Response<UserDto>> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken ctx = default
    ) => await userService.CreateAsync(request, ctx);

    [HttpPut("{id:guid}")]
    public async Task<Response<bool>> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken ctx = default
    )
    {
        request.Id = id;
        return await userService.UpdateAsync(request, ctx);
    }

    [HttpDelete]
    public async Task<Response<bool>> Delete(
        [FromBody] DeleteRequest request,
        CancellationToken ctx = default
    ) => await userService.DeleteAsync(request.Id, ctx);

    [HttpPost("group")]
    public async Task<Response<bool>> AddToGroup(
        [FromBody] AddUserToGroupRequest request,
        CancellationToken ctx = default
    ) => await userService.AddUserToGroupAsync(request.UserId, request.GroupId, ctx);

    [HttpDelete("group")]
    public async Task<Response<bool>> RemoveFromGroup(
        [FromBody] AddUserToGroupRequest request,
        CancellationToken ctx = default
    ) => await userService.RemoveUserFromGroupAsync(request.UserId, request.GroupId, ctx);

    [HttpGet("permissions")]
    [Auth("SU", true, false)]
    public async Task<Response<IEnumerable<PermissionDto>>> GetUserPermissions(
        [FromQuery] Guid userId,
        CancellationToken ctx = default
    ) => await userService.GetUserPermissionsAsync(userId, ctx);

    [HttpPost("permission")]
    public async Task<Response<bool>> AddPermissionToUser(
        [FromBody] AddUserPermissionRequest request,
        CancellationToken ctx = default
    ) =>
        await userService.AddPermissionToUserAsync(
            request.UserId,
            request.PermissionId,
            request.GroupId,
            ctx
        );

    [HttpDelete("permission")]
    public async Task<Response<bool>> RemovePermissionFromUser(
        [FromBody] AddUserPermissionRequest request,
        CancellationToken ctx = default
    ) =>
        await userService.RemovePermissionFromUserAsync(
            request.UserId,
            request.PermissionId,
            request.GroupId,
            ctx
        );

    [HttpGet("{userId:guid}/groups")]
    public async Task<Response<IEnumerable<GroupSimpleDto>>> GetUserGroups(
        [FromRoute] Guid userId,
        CancellationToken ctx = default
    ) => await userService.GetUserGroupsAsync(userId, ctx);
}

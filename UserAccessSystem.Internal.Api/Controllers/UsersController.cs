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
        [FromQuery] DateTime? lastEntry
    ) => await userService.GetAllUsersAsync(lastEntry);

    [HttpPost]
    public async Task<Response<UserDto>> Create([FromBody] CreateUserRequest request) =>
        await userService.CreateAsync(request);

    [HttpPut]
    public async Task<Response<bool>> Update([FromBody] CreateUserRequest request) =>
        await userService.UpdateAsync(request);

    [HttpDelete("{id:guid}")]
    public async Task<Response<bool>> Delete([FromRoute] Guid id) =>
        await userService.DeleteAsync(id);

    [HttpPost("{id:guid}/groups/{groupId:guid}")]
    public async Task<Response<bool>> AddToGroup([FromRoute] Guid id, [FromRoute] Guid groupId) =>
        await userService.AddUserToGroupAsync(id, groupId);

    [HttpDelete("{id:guid}/groups/{groupId:guid}")]
    public async Task<Response<bool>> RemoveFromGroup(
        [FromRoute] Guid id,
        [FromRoute] Guid groupId
    ) => await userService.RemoveUserFromGroupAsync(id, groupId);
}

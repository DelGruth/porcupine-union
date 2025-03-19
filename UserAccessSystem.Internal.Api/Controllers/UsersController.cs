using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;
using UserAccessSystem.Contract.Requests;
using UserAccessSystem.Contract.Responses;
using UserAccessSystem.Internal.Application.Infrastructure;

namespace UserAccessSystem.Internal.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet()]
    [Route("{id:guid}")]
    [AllowAnonymous]
    public Task<Response<GetUserResponse>> Get([FromRoute] Guid id)
    {
        return Task.FromResult(
            new Response<GetUserResponse>(new GetUserResponse() { Name = id.ToString() })
        );
    }

    [HttpGet()]
    [AllowAnonymous]
    public async ValueTask<Response<IEnumerable<UserDto>>> Get([FromQuery] DateTime? lastEntry) =>
        await userService.GetAllUsersAsync(lastEntry);

    [HttpPost()]
    [AllowAnonymous]
    public async ValueTask<Response<UserDto>> Post([FromBody] CreateUserRequest request) =>
        await userService.CreateAsync(request);
}

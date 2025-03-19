using Microsoft.AspNetCore.Mvc;
using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;
using UserAccessSystem.Contract.Responses;
using UserAccessSystem.Internal.Application.Infrastructure;

namespace UserAccessSystem.Internal.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet()]
    [Route("{id:guid}")]
    public Task<Response<GetUserResponse>> Get([FromRoute] Guid id)
    {
        return Task.FromResult(
            new Response<GetUserResponse>(new GetUserResponse() { Name = id.ToString() })
        );
    }

    [HttpGet()]
    public ValueTask<Response<IEnumerable<UserDto>>> Get()
    {
        return userService.GetAllUsers(null);
    }

    [HttpGet()]
    public ValueTask<Response<IEnumerable<UserDto>>> Get([FromQuery] DateTime? lastEntry)
    {
        return userService.GetAllUsers(lastEntry);
    }
}

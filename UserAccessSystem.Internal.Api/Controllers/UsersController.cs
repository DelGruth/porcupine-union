using Microsoft.AspNetCore.Mvc;
using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Responses;

namespace UserAccessSystem.Internal.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
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
    public Task<Response<GetAllUsersResponse>> Get()
    {
        return Task.FromResult(
            new Response<GetAllUsersResponse>(
                new GetAllUsersResponse()
                {
                    Names = Enumerable
                        .Range(1, 19)
                        .Select(x => Guid.NewGuid().ToString())
                        .ToArray(),
                }
            )
        );
    }
}

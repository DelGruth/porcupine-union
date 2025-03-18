using Microsoft.AspNetCore.Mvc;
using UserAccessSystem.Contract;

namespace UserAccessSystem.Internal.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet()]
    [Route("{count}")]
    public Task<Response<IEnumerable<GetUserResponse>>> Get(int count)
    {
        return Task.FromResult(
            new Response<IEnumerable<GetUserResponse>>(
                Enumerable
                    .Range(1, 5)
                    .Select(index => new GetUserResponse() { Name = $"User {index}" })
            )
        );
    }
}

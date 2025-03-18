using Microsoft.AspNetCore.Mvc;
using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Responses;

namespace UserAccessSystem.Internal.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GroupsController : ControllerBase
{
    [HttpGet()]
    [Route("{id:guid}")]
    public Task<Response<GetGroupResponse>> Get([FromRoute] Guid id)
    {
        return Task.FromResult(
            new Response<GetGroupResponse>(new GetGroupResponse() { GroupName = id.ToString() })
        );
    }

    [HttpGet()]
    public Task<Response<GetAllGroupsResponse>> Get()
    {
        return Task.FromResult(
            new Response<GetAllGroupsResponse>(
                new GetAllGroupsResponse()
                {
                    GroupNames = Enumerable
                        .Range(1, 19)
                        .Select(x => Guid.NewGuid().ToString())
                        .ToArray(),
                }
            )
        );
    }
}

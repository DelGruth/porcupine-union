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
public class PermissionsController(IPermissionService permissionService) : ControllerBase
{
    [HttpGet]
    public async ValueTask<Response<IEnumerable<PermissionDto>>> GetAll() =>
        await permissionService.GetAllPermissionsAsync();

    [HttpPost]
    public async Task<Response<PermissionDto>> Create([FromBody] CreatePermissionRequest request) =>
        await permissionService.CreateAsync(request);

    [HttpDelete("{id:guid}")]
    public async Task<Response<bool>> Delete([FromRoute] Guid id) =>
        await permissionService.DeleteAsync(id);
}

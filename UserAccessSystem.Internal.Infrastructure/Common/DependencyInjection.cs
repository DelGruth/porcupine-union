using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserAccessSystem.Internal.Application.Infrastructure;
using UserAccessSystem.Internal.Infrastructure.Service;

namespace UserAccessSystem.Internal.Infrastructure.Common;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInfrastrucutre(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHybridCache();
        builder.Services.AddScoped<IGroupService, GroupService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IPermissionService, PermissionService>();
        return builder;
    }
}

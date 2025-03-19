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
        builder.Services.AddSingleton<IGroupService, GroupService>();
        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddSingleton<IPermissionService, PermissionService>();
        return builder;
    }
}

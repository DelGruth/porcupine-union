using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserAccessSystem.Internal.Application.Peristence;
using UserAccessSystem.Internal.Persistence.DbContext;
using UserAccessSystem.Internal.Persistence.Interfaces;
using UserAccessSystem.Internal.Persistence.Repository;

namespace UserAccessSystem.Internal.Persistence.Common;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddPersistence(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<UserAccessDbContext>(config =>
            config.UseInMemoryDatabase("mainDb").EnableSensitiveDataLogging().EnableDetailedErrors()
        );

        builder.Services.AddDistributedMemoryCache();

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
        builder.Services.AddScoped<IGroupRepository, GroupRepository>();

        return builder;
    }

    public static WebApplication BuildPersistence(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserAccessDbContext>();
        dbContext.Database.EnsureCreated();

        return app;
    }
}

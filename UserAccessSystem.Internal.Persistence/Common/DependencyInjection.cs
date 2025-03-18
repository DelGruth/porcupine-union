using Microsoft.Extensions.Hosting;

namespace UserAccessSystem.Internal.Persistence.Common;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddPersistence(this IHostApplicationBuilder builder)
    {
        return builder;
    }
}

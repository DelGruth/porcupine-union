using Microsoft.Extensions.Hosting;

namespace UserAccessSystem.Internal.Infrastructure.Common;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInfrastrucutre(this IHostApplicationBuilder builder)
    {
        return builder;
    }
}

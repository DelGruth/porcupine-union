using Microsoft.Extensions.Hosting;

namespace UserAccessSystem.Internal.Application.Common;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddApplication(this IHostApplicationBuilder builder)
    {
        return builder;
    }
}

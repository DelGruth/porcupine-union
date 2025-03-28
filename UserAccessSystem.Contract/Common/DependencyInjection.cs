using Microsoft.Extensions.DependencyInjection;
using UserAccessSystem.Contract.Clients;

namespace UserAccessSystem.Contract.Common;

public static class DependencyInjection
{
    public static IServiceCollection AddUserPermissionsClient(
        this IServiceCollection services,
        string baseUrl
    )
    {
        services.AddHttpClient(
            ApiClient.CLIENT_NAME,
            client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            }
        );

        services.AddScoped<ApiClient>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient(ApiClient.CLIENT_NAME);
            return new ApiClient(client);
        });

        return services;
    }
}

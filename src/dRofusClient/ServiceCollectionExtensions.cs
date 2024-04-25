using Microsoft.Extensions.DependencyInjection;

namespace dRofusClient;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDRofusClient(IServiceCollection services)
    {
        services.AddHttpClient<IdRofusClient, dRofusClient>();
        return services;
    }
}
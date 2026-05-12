using IndexedDB.NET.Core;
using Microsoft.Extensions.DependencyInjection;

namespace IndexedDB.NET.Blazor;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIndexedDb(this IServiceCollection services, Action<IndexedDbOptions> configure)
    {
        var options = new IndexedDbOptions();

        configure(options);

        services.AddSingleton(options);

        return services;
    }
}
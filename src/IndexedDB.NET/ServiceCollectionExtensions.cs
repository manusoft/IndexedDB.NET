using ManuHub.IndexedDB.Context;
using Microsoft.Extensions.DependencyInjection;

namespace ManuHub.IndexedDB;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers IndexedDbContext with default options
    /// </summary>
    public static IServiceCollection AddIndexedDb<TContext>(this IServiceCollection services)
        where TContext : IndexedDbContext
    {
        return services.AddIndexedDb<TContext>(_ => { });
    }

    /// <summary>
    /// Registers IndexedDbContext with custom options configuration
    /// </summary>
    public static IServiceCollection AddIndexedDb<TContext>(
        this IServiceCollection services,
        Action<IndexedDbOptions> configure)
        where TContext : IndexedDbContext
    {
        services.AddScoped<IndexedDbOptions>(provider =>
        {
            var options = new IndexedDbOptions();
            configure(options);
            return options;
        });

        services.AddScoped<TContext>(provider =>
        {
            var options = provider.GetRequiredService<IndexedDbOptions>();
            var context = ActivatorUtilities.CreateInstance<TContext>(provider, options);

            context.Services = provider;

            return context;
        });

        return services;
    }
}
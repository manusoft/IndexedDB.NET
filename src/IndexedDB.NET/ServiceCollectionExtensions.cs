using ManuHub.IndexedDB.Context;
using Microsoft.Extensions.DependencyInjection;

namespace ManuHub.IndexedDB;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIndexedDb<TContext>(this IServiceCollection services)
       where TContext : IndexedDbContext
    {
        services.AddScoped<TContext>(provider =>
        {
            var context = ActivatorUtilities.CreateInstance<TContext>(provider);

            context.Services = provider;

            return context;
        });

        return services;
    }
}
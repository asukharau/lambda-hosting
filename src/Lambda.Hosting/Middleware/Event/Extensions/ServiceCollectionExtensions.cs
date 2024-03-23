using Microsoft.Extensions.DependencyInjection;

namespace Lambda.Hosting;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddLoggingMiddleware<TEvent>(this IServiceCollection services)
    {
        services.AddScoped<IFunctionMiddleware, FunctionLoggingMiddleware<TEvent>>();
        return services;
    }

    public static IServiceCollection AddMiddleware<TEvent, TMiddleware>(this IServiceCollection services)
        where TMiddleware : FunctionMiddlewareBase<TEvent>, IFunctionMiddleware
    {
        services.AddScoped<IFunctionMiddleware, TMiddleware>();
        return services;
    }
    
    public static IServiceCollection AddMiddleware<TEvent>(this IServiceCollection services, Func<TEvent, CancellationToken, Task> handler)
    {
        services.AddScoped<IFunctionMiddleware>(_ => new FunctionMiddleware<TEvent>(handler));
        return services;
    }

    public static IServiceCollection AddMiddleware<TEvent>(this IServiceCollection services, Func<IServiceProvider, TEvent, CancellationToken, Task> handler)
    {
        services.AddScoped<IFunctionMiddleware>(serviceProvider =>
        {
            return new FunctionMiddleware<TEvent>(Handler);

            async Task Handler(TEvent @event, CancellationToken cancellationToken)
            {
                await handler(serviceProvider, @event, cancellationToken).ConfigureAwait(false);
            }
        });

        return services;
    }
}
using Microsoft.Extensions.DependencyInjection;

namespace Lambda.Hosting;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddLoggingMiddleware<TRequest, TResponse>(this IServiceCollection services)
    {
        services.AddScoped<IFunctionMiddleware, FunctionLoggingMiddleware<TRequest, TResponse>>();
        return services;
    }

    public static IServiceCollection AddMiddleware<TRequest, TResponse, TMiddleware>(this IServiceCollection services)
        where TMiddleware : FunctionMiddlewareBase<TRequest, TResponse>, IFunctionMiddleware
    {
        services.AddScoped<IFunctionMiddleware, TMiddleware>();
        return services;
    }

    public static IServiceCollection AddMiddleware<TRequest, TResponse>(this IServiceCollection services, Func<TRequest, CancellationToken, Task<TResponse>> handler)
    {
        services.AddScoped<IFunctionMiddleware>(_ => new FunctionMiddleware<TRequest, TResponse>(handler));
        return services;
    }

    public static IServiceCollection AddMiddleware<TRequest, TResponse>(this IServiceCollection services, Func<IServiceProvider, TRequest, CancellationToken, Task<TResponse>> handler)
    {
        services.AddScoped<IFunctionMiddleware>(serviceProvider =>
        {
            return new FunctionMiddleware<TRequest, TResponse>(Handler);

            async Task<TResponse> Handler(TRequest request, CancellationToken cancellationToken)
            {
                return await handler(serviceProvider, request, cancellationToken).ConfigureAwait(false);
            }
        });

        return services;
    }
}
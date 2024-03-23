using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Lambda.Hosting;

public abstract class FunctionHandler<TRequest, TResponse> : FunctionHandlerBase
{
    public async Task<TResponse> HandleAsync(TRequest request, ILambdaContext lambdaContext)
    {
        using var cancellationTokenSource = new CancellationTokenSource(lambdaContext.RemainingTime);

        if (!IsInitialized)
        {
            await InitializeAsync(lambdaContext, cancellationTokenSource.Token).ConfigureAwait(false);
        }

        return await HandleRequestAsync(request, lambdaContext, cancellationTokenSource.Token).ConfigureAwait(false);
    }

    private async Task<TResponse> HandleRequestAsync(TRequest request, ILambdaContext lambdaContext, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var serviceScopeFactory = Host?.Services.GetRequiredService<IServiceScopeFactory>();
        if (serviceScopeFactory is null)
        {
            throw new InvalidOperationException();
        }

        await using var scope = serviceScopeFactory.CreateAsyncScope();

        var middlewares = scope.ServiceProvider.GetServices<IFunctionMiddleware>();

        var pipeline = new FunctionMiddlewarePipeline<TRequest, TResponse>();
        foreach (var middleware in middlewares)
        {
            pipeline.AddMiddleware(middleware);
        }

        var functionHandlerContext = new FunctionHandlerContext<TRequest, TResponse>
        {
            Request = request
        };

        await pipeline.ExecuteAsync(functionHandlerContext, lambdaContext, cancellationToken).ConfigureAwait(false);
        if (functionHandlerContext.Response is null)
        {
            throw new InvalidOperationException();
        }

        return functionHandlerContext.Response;
    }
}
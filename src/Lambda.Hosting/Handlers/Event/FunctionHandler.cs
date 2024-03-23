using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Lambda.Hosting;

public abstract class FunctionHandler<TEvent> : FunctionHandlerBase
{
    public async Task HandleAsync(TEvent @event, ILambdaContext lambdaContext)
    {
        using var cancellationTokenSource = new CancellationTokenSource(lambdaContext.RemainingTime);

        if (!IsInitialized)
        {
            await InitializeAsync(lambdaContext, cancellationTokenSource.Token).ConfigureAwait(false);
        }

        await HandleEventAsync(@event, lambdaContext, cancellationTokenSource.Token).ConfigureAwait(false);
    }

    private async Task HandleEventAsync(TEvent @event, ILambdaContext lambdaContext, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var serviceScopeFactory = Host?.Services.GetRequiredService<IServiceScopeFactory>();
        if (serviceScopeFactory is null)
        {
            throw new InvalidOperationException();
        }

        await using var scope = serviceScopeFactory.CreateAsyncScope();

        var middlewares = scope.ServiceProvider.GetServices<IFunctionMiddleware>();

        var pipeline = new FunctionMiddlewarePipeline<TEvent>();
        foreach (var middleware in middlewares)
        {
            pipeline.AddMiddleware(middleware);
        }

        var functionHandlerContext = new FunctionHandlerContext<TEvent>
        {
            Event = @event
        };

        await pipeline.ExecuteAsync(functionHandlerContext, lambdaContext, cancellationToken).ConfigureAwait(false);
    }
}
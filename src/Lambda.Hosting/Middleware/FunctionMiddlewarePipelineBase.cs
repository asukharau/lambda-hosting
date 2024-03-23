using Amazon.Lambda.Core;

namespace Lambda.Hosting;

internal abstract class FunctionMiddlewarePipelineBase : IFunctionMiddlewarePipeline
{
    private readonly List<IFunctionMiddleware> _middlewares = new();

    public void AddMiddleware(IFunctionMiddleware middleware)
    {
        _middlewares.Add(middleware);
    }

    public Task ExecuteAsync(IFunctionHandlerContext functionHandlerContext, ILambdaContext lambdaContext, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        FunctionMiddlewareDelegate? app = null;

        for (var i = _middlewares.Count - 1; i >= 0; --i)
        {
            app = ChainMiddleware(_middlewares[i], app);
        }

        if (app == null)
        {
            throw new InvalidOperationException();
        }

        return app(functionHandlerContext, lambdaContext, cancellationToken);
    }

    protected abstract FunctionMiddlewareDelegate ChainMiddleware(IFunctionMiddleware middleware, FunctionMiddlewareDelegate? next);
}
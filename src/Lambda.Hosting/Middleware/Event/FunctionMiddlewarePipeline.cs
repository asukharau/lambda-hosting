namespace Lambda.Hosting;

internal sealed class FunctionMiddlewarePipeline<TEvent> : FunctionMiddlewarePipelineBase
{
    protected override FunctionMiddlewareDelegate ChainMiddleware(IFunctionMiddleware middleware, FunctionMiddlewareDelegate? next)
    {
        return async (functionHandlerContext, lambdaContext, cancellationToken) =>
        {
            await middleware.InvokeAsync(functionHandlerContext, lambdaContext, next, cancellationToken).ConfigureAwait(false);
        };
    }
}
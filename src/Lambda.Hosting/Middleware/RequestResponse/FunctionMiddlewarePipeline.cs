namespace Lambda.Hosting;

internal sealed class FunctionMiddlewarePipeline<TRequest, TResponse> : FunctionMiddlewarePipelineBase
{
    protected override FunctionMiddlewareDelegate ChainMiddleware(IFunctionMiddleware middleware, FunctionMiddlewareDelegate? next)
    {
        return async (functionHandlerContext, lambdaContext, cancellationToken) =>
        {
            if (functionHandlerContext is FunctionHandlerContext<TRequest, TResponse> {Response: not null})
            {
                return;
            }

            await middleware.InvokeAsync(functionHandlerContext, lambdaContext, next, cancellationToken).ConfigureAwait(false);
        };
    }
}
using Amazon.Lambda.Core;

namespace Lambda.Hosting;

public abstract class FunctionMiddlewareBase<TRequest, TResponse> : IFunctionMiddleware
{
    Task IFunctionMiddleware.InvokeAsync(IFunctionHandlerContext functionHandlerContext, ILambdaContext lambdaContext, FunctionMiddlewareDelegate? next, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (functionHandlerContext is not FunctionHandlerContext<TRequest, TResponse> typedFunctionHandlerContext)
        {
            throw new NotSupportedException();
        }

        return InvokeAsync(typedFunctionHandlerContext, lambdaContext, next, cancellationToken);
    }

    protected abstract Task InvokeAsync(FunctionHandlerContext<TRequest, TResponse> functionHandlerContext, ILambdaContext lambdaContext, FunctionMiddlewareDelegate? next, CancellationToken cancellationToken);
}
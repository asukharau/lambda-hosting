using Amazon.Lambda.Core;

namespace Lambda.Hosting;

public abstract class FunctionMiddlewareBase<TEvent> : IFunctionMiddleware
{
    Task IFunctionMiddleware.InvokeAsync(IFunctionHandlerContext functionHandlerContext, ILambdaContext lambdaContext, FunctionMiddlewareDelegate? next, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (functionHandlerContext is not FunctionHandlerContext<TEvent> typedFunctionHandlerContext)
        {
            throw new NotSupportedException();
        }

        return InvokeAsync(typedFunctionHandlerContext, lambdaContext, next, cancellationToken);
    }
    
    protected abstract Task InvokeAsync(FunctionHandlerContext<TEvent> functionHandlerContext, ILambdaContext lambdaContext, FunctionMiddlewareDelegate? next, CancellationToken cancellationToken);
}
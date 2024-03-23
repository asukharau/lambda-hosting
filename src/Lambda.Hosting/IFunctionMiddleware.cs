using Amazon.Lambda.Core;

namespace Lambda.Hosting;

public interface IFunctionMiddleware
{
    Task InvokeAsync(IFunctionHandlerContext functionHandlerContext, ILambdaContext lambdaContext, FunctionMiddlewareDelegate? next, CancellationToken cancellationToken);
}
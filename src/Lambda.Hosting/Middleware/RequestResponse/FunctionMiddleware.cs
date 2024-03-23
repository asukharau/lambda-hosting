using Amazon.Lambda.Core;

namespace Lambda.Hosting;

internal sealed class FunctionMiddleware<TRequest, TResponse> : FunctionMiddlewareBase<TRequest, TResponse>
{
    private readonly Func<TRequest, CancellationToken, Task<TResponse>> _func;

    public FunctionMiddleware(Func<TRequest, CancellationToken, Task<TResponse>> func)
    {
        _func = func;
    }

    protected override async Task InvokeAsync(FunctionHandlerContext<TRequest, TResponse> functionHandlerContext, ILambdaContext lambdaContext, FunctionMiddlewareDelegate? next, CancellationToken cancellationToken)
    {
        var request = functionHandlerContext.Request;
        if (request is null)
        {
            throw new InvalidOperationException();
        }

        functionHandlerContext.Response = await _func(request, cancellationToken).ConfigureAwait(false);

        if (next is not null)
        {
            await next(functionHandlerContext, lambdaContext, cancellationToken).ConfigureAwait(false);
        }
    }
}
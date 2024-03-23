using Amazon.Lambda.Core;

namespace Lambda.Hosting;

internal sealed class FunctionMiddleware<TEvent> : FunctionMiddlewareBase<TEvent>
{
    private readonly Func<TEvent, CancellationToken, Task> _action;

    public FunctionMiddleware(Func<TEvent, CancellationToken, Task> action)
    {
        _action = action;
    }

    protected override async Task InvokeAsync(FunctionHandlerContext<TEvent> functionHandlerContext, ILambdaContext lambdaContext, FunctionMiddlewareDelegate? next, CancellationToken cancellationToken)
    {
        var @event = functionHandlerContext.Event;
        if (@event is null)
        {
            throw new InvalidOperationException();
        }

        await _action(@event, cancellationToken).ConfigureAwait(false);

        if (next is not null)
        {
            await next(functionHandlerContext, lambdaContext, cancellationToken).ConfigureAwait(false);
        }
    }
}

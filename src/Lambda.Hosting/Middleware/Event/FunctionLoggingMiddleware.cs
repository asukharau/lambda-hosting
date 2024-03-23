using System.Diagnostics;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Logging;

namespace Lambda.Hosting;

internal sealed class FunctionLoggingMiddleware<TRequest> : FunctionMiddlewareBase<TRequest>
{
    private readonly ILogger _logger;

    public FunctionLoggingMiddleware(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger("FunctionLoggingMiddleware");
    }

    protected override async Task InvokeAsync(FunctionHandlerContext<TRequest> functionHandlerContext, ILambdaContext lambdaContext, FunctionMiddlewareDelegate? next, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        _logger.EventProcessingStarted(functionHandlerContext.Event);

        try
        {
            if (next is not null)
            {
                await next(functionHandlerContext, lambdaContext, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            _logger.UnhandledExceptionOccured(e);
            throw;
        }
        finally
        {
            _logger.EventProcessingFinished(functionHandlerContext.Event, stopwatch.Elapsed);
        }
    }
}
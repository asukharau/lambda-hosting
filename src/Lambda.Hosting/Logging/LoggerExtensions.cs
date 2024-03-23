using Microsoft.Extensions.Logging;

namespace Lambda.Hosting;

internal static partial class LoggerExtensions
{
    [LoggerMessage(1, LogLevel.Error, "An unhandled exception has occurred while execution", EventName = "UnhandledExceptionOccured")]
    public static partial void UnhandledExceptionOccured(this ILogger logger, Exception exception);

    [LoggerMessage(2, LogLevel.Information, "Request processing started", EventName = "RequestProcessingStarted")]
    public static partial void RequestProcessingStarted(this ILogger logger);

    [LoggerMessage(3, LogLevel.Information, "Request processing finished in {ElapsedTimeInMilliseconds}ms")]
    public static partial void RequestProcessingFinished(this ILogger logger, double elapsedTimeInMilliseconds);

    [LoggerMessage(4, LogLevel.Information, "Event processing started", EventName = "EventProcessingStarted")]
    public static partial void EventProcessingStarted(this ILogger logger);

    [LoggerMessage(5, LogLevel.Information, "Event processing finished in {ElapsedTimeInMilliseconds}ms")]
    public static partial void EventProcessingFinished(this ILogger logger, double elapsedTimeInMilliseconds);

    public static void RequestProcessingStarted<TRequest>(this ILogger logger, TRequest? request)
    {
        RequestProcessingStarted(logger);
    }

    public static void RequestProcessingFinished<TRequest, TResponse>(this ILogger logger, TRequest? request, TResponse? response, TimeSpan elapsedTime)
    {
        RequestProcessingFinished(logger, elapsedTime.TotalMilliseconds);
    }

    public static void EventProcessingStarted<TEvent>(this ILogger logger, TEvent? @event)
    {
        EventProcessingStarted(logger);
    }

    public static void EventProcessingFinished<TEvent>(this ILogger logger, TEvent? @event, TimeSpan elapsedTime)
    {
        EventProcessingFinished(logger, elapsedTime.TotalMilliseconds);
    }
}
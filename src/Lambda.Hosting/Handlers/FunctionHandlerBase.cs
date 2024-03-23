using Amazon.Lambda.Core;
using Microsoft.Extensions.Hosting;

namespace Lambda.Hosting;

public abstract class FunctionHandlerBase : IAsyncDisposable, IDisposable
{
    protected IHost? Host { get; private set; }
    
    protected bool IsInitialized { get; private set; }

    protected async Task InitializeAsync(ILambdaContext lambdaContext, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Host = CreateHostBuilder(lambdaContext).Build();

        await Host.StartAsync(cancellationToken).ConfigureAwait(false);

        IsInitialized = true;
    }

    protected virtual IHostBuilder CreateHostBuilder(ILambdaContext lambdaContext)
    {
        return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Host?.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (Host is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else
        {
            Host?.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        GC.SuppressFinalize(this);
    }
}

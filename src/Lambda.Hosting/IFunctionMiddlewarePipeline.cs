using Amazon.Lambda.Core;

namespace Lambda.Hosting;

public interface IFunctionMiddlewarePipeline
{
    Task ExecuteAsync(IFunctionHandlerContext functionHandlerContext, ILambdaContext lambdaContext, CancellationToken cancellationToken);
}
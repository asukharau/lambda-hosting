using Amazon.Lambda.Core;

namespace Lambda.Hosting;

public delegate Task FunctionMiddlewareDelegate(IFunctionHandlerContext functionHandlerContext, ILambdaContext lambdaContext, CancellationToken cancellationToken);
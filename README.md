# AWS Lambda + Microsoft.Extensions.Hosting.IHost = ❤️

## Intro

This project introduces a simplified approach to AWS Lambda development in C# by seamlessly integrating with `Microsoft.Extensions.Hosting.IHost` and incorporating middleware-like pipeline support reminiscent of ASP.NET Core. Whether you're crafting event-driven asynchronous Lambdas or request-response synchronous models, you can harness the familiar concept of middleware.

## Example

Below, you can find an example of how to create an AWS Lambda using the library for the [OS-only](https://docs.aws.amazon.com/lambda/latest/dg/runtimes-provided.html) runtime:

#### Program.cs
```
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;

namespace HelloWorld;

public static class Program
{
    public static async Task Main(string[] args)
    {
        await using var handler = new FunctionHandler();

        var serializer = new DefaultLambdaJsonSerializer();
        var bootstrapBuilder = LambdaBootstrapBuilder.Create<APIGatewayHttpApiV2ProxyRequest, APIGatewayHttpApiV2ProxyResponse>(
            handler.HandleAsync, serializer
        );

        using var bootstrap = bootstrapBuilder.Build();
        await bootstrap.RunAsync();
    }
}
```

#### FunctionHandler.cs
```
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Lambda.Hosting;
using Microsoft.Extensions.Hosting;

namespace HelloWorld;

internal sealed class FunctionHandler : FunctionHandler<APIGatewayHttpApiV2ProxyRequest, APIGatewayHttpApiV2ProxyResponse>
{
    protected override IHostBuilder CreateHostBuilder(ILambdaContext lambdaContext)
    {
        var hostBuilder = base.CreateHostBuilder(lambdaContext)
            .ConfigureServices(services =>
            {
                services.AddLoggingMiddleware<APIGatewayHttpApiV2ProxyRequest, APIGatewayHttpApiV2ProxyResponse>();

                services.AddMiddleware<APIGatewayHttpApiV2ProxyRequest, APIGatewayHttpApiV2ProxyResponse>(
                    (request, cancellationToken) =>
                    {
                        var response = new APIGatewayHttpApiV2ProxyResponse
                        {
                            StatusCode = 200,
                            Body = """
                                   {
                                       "message": "Hello World!"
                                   }
                                   """
                        };

                        return Task.FromResult(response);
                    } 
                );
            });

        return hostBuilder;
    }
}
```
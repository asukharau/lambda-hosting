namespace Lambda.Hosting;

public class FunctionHandlerContext<TRequest, TResponse> : IFunctionHandlerContext
{
    public TRequest? Request { get; set; }

    public TResponse? Response { get; set; }
}
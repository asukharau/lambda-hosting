namespace Lambda.Hosting;

public class FunctionHandlerContext<TEvent> : IFunctionHandlerContext
{
    public TEvent? Event { get; set; }
}
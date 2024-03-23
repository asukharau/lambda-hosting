using System.Text.Json;
using Amazon.Lambda.Serialization.SystemTextJson;

namespace Lambda.Hosting;

public sealed class FunctionJsonSerializer : DefaultLambdaJsonSerializer
{
    public FunctionJsonSerializer()
        : base(jsonSerializerOptions =>
            {
                jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                jsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            }
        )
    {
    }
}
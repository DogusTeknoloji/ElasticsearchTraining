using Nest;

namespace ElasticTraining.Models;

public class InnerExceptionDetails
{
    [Keyword(Name = "type")]
    public string Type { get; set; } = string.Empty;

    [Text(Name = "message")]
    public string Message { get; set; } = string.Empty;

    [Text(Index = false, Name = "stack_trace")]
    public string StackTrace { get; set; } = string.Empty;
}

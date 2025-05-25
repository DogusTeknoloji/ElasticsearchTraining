using Nest;

namespace ElasticTraining.Models;

public class ApplicationLog
{
    [Date(Name = "@timestamp")]
    public DateTime Timestamp { get; set; }

    [Keyword]
    public string? CorrelationId { get; set; }

    [Keyword]
    public string ServiceName { get; set; } = string.Empty;

    [Keyword]
    public string Level { get; set; } = string.Empty;

    [Keyword]
    public string ThreadName { get; set; } = string.Empty;

    [Keyword]
    public string LoggerName { get; set; } = string.Empty;

    [Ip]
    public string HostIp { get; set; } = string.Empty;

    [Text]
    public string Message { get; set; } = string.Empty;

    [Object]
    public ExceptionDetails? Exception { get; set; }

    [Number(NumberType.Integer)]
    public int? HttpStatusCode { get; set; }

    [Number(NumberType.Long)]
    public long? ResponseTimeMs { get; set; }
}

public class ExceptionDetails
{
    [Keyword]
    public string Type { get; set; } = string.Empty;

    [Text]
    public string Message { get; set; } = string.Empty;

    [Text(Index = false)]
    public string StackTrace { get; set; } = string.Empty;

    [Object]
    public InnerExceptionDetails? InnerException { get; set; }
}

public class InnerExceptionDetails
{
    [Keyword]
    public string Type { get; set; } = string.Empty;

    [Text]
    public string Message { get; set; } = string.Empty;

    [Text(Index = false)]
    public string StackTrace { get; set; } = string.Empty;
}

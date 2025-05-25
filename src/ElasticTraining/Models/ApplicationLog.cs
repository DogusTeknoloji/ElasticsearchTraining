using Nest;

namespace ElasticTraining.Models;

public class ApplicationLog
{
    [Date(Name = "@timestamp")]
    public DateTime Timestamp { get; set; }

    [Keyword(Name = "correlation_id")]
    public string? CorrelationId { get; set; }

    [Keyword(Name = "service_name")]
    public string ServiceName { get; set; } = string.Empty;

    [Keyword(Name = "level")]
    public string Level { get; set; } = string.Empty;

    [Keyword(Name = "thread_name")]
    public string ThreadName { get; set; } = string.Empty;

    [Keyword(Name = "logger_name")]
    public string LoggerName { get; set; } = string.Empty;

    [Ip(Name = "host_ip")]
    public string HostIp { get; set; } = string.Empty;

    [Text(Name = "message")]
    public string Message { get; set; } = string.Empty;

    [Object(Name = "exception")]
    public ExceptionDetails? Exception { get; set; }

    [Number(NumberType.Integer, Name = "http_status_code")]
    public int? HttpStatusCode { get; set; }

    [Number(NumberType.Long, Name = "response_time_ms")]
    public long? ResponseTimeMs { get; set; }
}

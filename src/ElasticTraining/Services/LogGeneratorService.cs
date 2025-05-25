using ElasticTraining.Models;
using Nest;

namespace ElasticTraining.Services;

public class LogGeneratorService : BackgroundService, ILogGeneratorService
{
    private readonly IElasticClient _client;
    private readonly ILogger<LogGeneratorService> _logger;
    private readonly Random _random = new();
    
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isRunning;

    private readonly string[] _services = { "auth-service", "user-service", "product-catalog", "order-service", "payment-service", "notification-service" };
    private readonly string[] _logLevels = { "INFO", "WARN", "ERROR" };
    private readonly string[] _loggerNames = { "com.company.auth.AuthController", "com.company.user.UserService", "com.company.product.ProductController", "com.company.order.OrderService" };
    private readonly string[] _threadNames = { "http-nio-8080-exec-1", "http-nio-8080-exec-2", "scheduler-thread-1", "async-task-executor-1" };
    
    private readonly string[] _infoMessages = 
    {
        "User logged in successfully",
        "Product catalog refreshed",
        "Order created successfully",
        "Payment processed",
        "Email notification sent",
        "Database connection pool initialized",
        "Cache updated successfully",
        "Scheduled task completed"
    };
    
    private readonly string[] _warnMessages = 
    {
        "High memory usage detected",
        "Slow database query detected",
        "API rate limit approaching",
        "Cache miss rate is high",
        "Connection pool nearly exhausted",
        "Deprecated API endpoint accessed",
        "Configuration value missing, using default"
    };
    
    private readonly string[] _errorMessages = 
    {
        "Database connection timeout",
        "Authentication failed for user",
        "Payment processing failed",
        "External API call failed",
        "File processing error",
        "Validation error occurred",
        "Unable to send notification"
    };

    private readonly string[] _hostIps = { "192.168.1.10", "192.168.1.11", "192.168.1.12", "192.168.1.13", "192.168.1.14" };

    public LogGeneratorService(IElasticClient client, ILogger<LogGeneratorService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public bool IsRunning => _isRunning;

    public Task StartAsync()
    {
        if (_isRunning) return Task.CompletedTask;
        
        _cancellationTokenSource = new CancellationTokenSource();
        _isRunning = true;
        
        _logger.LogInformation("Log üretimi başlatıldı");
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        if (!_isRunning) return Task.CompletedTask;
        
        _cancellationTokenSource?.Cancel();
        _isRunning = false;
        
        _logger.LogInformation("Log üretimi durduruldu");
        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_isRunning)
            {
                await Task.Delay(1000, stoppingToken);
                continue;
            }

            try
            {
                await GenerateAndSendLogAsync();
                
                // 2-10 saniye arasında rastgele bekleme
                var delay = _random.Next(2000, 10000);
                await Task.Delay(delay, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Log üretimi sırasında hata oluştu");
                await Task.Delay(5000, stoppingToken); // Hata durumunda 5 saniye bekle
            }
        }
    }

    private async Task GenerateAndSendLogAsync()
    {
        var logLevel = GetWeightedLogLevel();
        var log = GenerateLog(logLevel);
        
        var indexName = $"application_logs-{DateTime.UtcNow:yyyy-MM-dd}";
        
        var response = await _client.IndexAsync(log, i => i.Index(indexName));
        
        if (!response.IsValid)
        {
            _logger.LogError("Log gönderimi başarısız: {Error}", response.DebugInformation);
        }
    }

    private string GetWeightedLogLevel()
    {
        // INFO: %70, WARN: %20, ERROR: %10
        var chance = _random.Next(100);
        return chance switch
        {
            < 70 => "INFO",
            < 90 => "WARN",
            _ => "ERROR"
        };
    }

    private ApplicationLog GenerateLog(string level)
    {
        var service = _services[_random.Next(_services.Length)];
        var correlationId = Guid.NewGuid().ToString("N")[..8];
        
        var log = new ApplicationLog
        {
            Timestamp = DateTime.UtcNow,
            CorrelationId = correlationId,
            ServiceName = service,
            Level = level,
            ThreadName = _threadNames[_random.Next(_threadNames.Length)],
            LoggerName = _loggerNames[_random.Next(_loggerNames.Length)],
            HostIp = _hostIps[_random.Next(_hostIps.Length)],
            Message = GetMessageForLevel(level)
        };

        // HTTP status code ve response time ekle (bazı loglar için)
        if (_random.Next(100) < 60) // %60 ihtimalle HTTP bilgileri ekle
        {
            log.HttpStatusCode = level switch
            {
                "INFO" => GetRandomInfoStatusCode(),
                "WARN" => GetRandomWarnStatusCode(),
                "ERROR" => GetRandomErrorStatusCode(),
                _ => 200
            };
            
            log.ResponseTimeMs = level switch
            {
                "INFO" => _random.Next(50, 300),
                "WARN" => _random.Next(200, 1000),
                "ERROR" => _random.Next(1000, 5000),
                _ => _random.Next(50, 300)
            };
        }

        // ERROR seviyesindeki loglar için exception detayları ekle
        if (level == "ERROR" && _random.Next(100) < 80) // %80 ihtimalle exception ekle
        {
            log.Exception = GenerateExceptionDetails();
        }

        return log;
    }

    private string GetMessageForLevel(string level)
    {
        return level switch
        {
            "INFO" => _infoMessages[_random.Next(_infoMessages.Length)],
            "WARN" => _warnMessages[_random.Next(_warnMessages.Length)],
            "ERROR" => _errorMessages[_random.Next(_errorMessages.Length)],
            _ => "Unknown log message"
        };
    }

    private int GetRandomInfoStatusCode()
    {
        var codes = new[] { 200, 201, 202, 204 };
        return codes[_random.Next(codes.Length)];
    }

    private int GetRandomWarnStatusCode()
    {
        var codes = new[] { 400, 401, 403, 404, 429 };
        return codes[_random.Next(codes.Length)];
    }

    private int GetRandomErrorStatusCode()
    {
        var codes = new[] { 500, 502, 503, 504 };
        return codes[_random.Next(codes.Length)];
    }

    private ExceptionDetails GenerateExceptionDetails()
    {
        var exceptionTypes = new[]
        {
            "System.TimeoutException",
            "System.ArgumentNullException",
            "System.InvalidOperationException",
            "System.Data.SqlClient.SqlException",
            "System.Net.Http.HttpRequestException",
            "System.IO.FileNotFoundException"
        };

        var exception = new ExceptionDetails
        {
            Type = exceptionTypes[_random.Next(exceptionTypes.Length)],
            Message = "An error occurred during operation execution",
            StackTrace = GenerateStackTrace()
        };

        // %30 ihtimalle inner exception ekle
        if (_random.Next(100) < 30)
        {
            exception.InnerException = new InnerExceptionDetails
            {
                Type = "System.Exception",
                Message = "Inner exception occurred",
                StackTrace = GenerateStackTrace()
            };
        }

        return exception;
    }

    private string GenerateStackTrace()
    {
        return @"   at Company.Service.Method(String parameter) in /app/src/Service.cs:line 42
   at Company.Controller.Action() in /app/src/Controller.cs:line 15
   at Microsoft.AspNetCore.Mvc.Infrastructure.ActionMethodExecutor.ExecuteAsync()
   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.InvokeActionMethodAsync()";
    }
}

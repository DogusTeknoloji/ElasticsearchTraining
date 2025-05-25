namespace ElasticTraining.Services;

public interface ILogGeneratorService
{
    Task StartAsync();
    Task StopAsync();
    bool IsRunning { get; }
    Task GenerateHistoricalDataAsync(int daysBack = 30);
    bool IsGeneratingHistoricalData { get; }
}

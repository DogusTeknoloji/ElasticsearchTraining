using ElasticTraining.Models;

namespace ElasticTraining.Services;

public interface ILogGeneratorService
{
    Task StartAsync();
    Task StopAsync();
    bool IsRunning { get; }
}

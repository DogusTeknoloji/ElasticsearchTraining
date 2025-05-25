namespace ElasticTraining.Services;

public interface IElasticsearchService
{
    Task<bool> CreateApplicationLogsTemplateAsync();
    Task<bool> CreateProductsIndexAsync();
    Task<bool> LoadSampleProductsAsync();
    Task<bool> IsElasticsearchAvailableAsync();
    Task<string> GetClusterHealthAsync();
}

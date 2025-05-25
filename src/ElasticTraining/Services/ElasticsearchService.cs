using ElasticTraining.Models;
using Nest;
using Elasticsearch.Net;

namespace ElasticTraining.Services;

public class ElasticsearchService(
    IElasticClient client,
    ILogger<ElasticsearchService> logger)
    : IElasticsearchService
{
    private readonly IElasticClient _client = client;
    private readonly ILogger<ElasticsearchService> _logger = logger;

    public async Task<bool> IsElasticsearchAvailableAsync()
    {
        try
        {
            var response = await _client.PingAsync();
            return response.IsValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during Elasticsearch connection check");
            return false;
        }
    }

    public async Task<string> GetClusterHealthAsync()
    {
        try
        {
            var response = await _client.Cluster.HealthAsync();
            if (response.IsValid)
            {
                return $"Cluster: {response.ClusterName}, Status: {response.Status}, Nodes: {response.NumberOfNodes}, Data Nodes: {response.NumberOfDataNodes}";
            }
            return "Cluster health information could not be retrieved";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving cluster health information");
            return "Error: " + ex.Message;
        }
    }

    public async Task<bool> CreateApplicationLogsTemplateAsync()
    {
        try
        {
            // Using modern Composable Index Template (Elasticsearch 7.8+)
            var templateJson = @"{
                ""index_patterns"": [""application_logs-*""],
                ""priority"": 1,
                ""template"": {
                    ""mappings"": {
                        ""dynamic"": false,
                        ""properties"": {
                            ""@timestamp"": { ""type"": ""date"" },
                            ""correlation_id"": { ""type"": ""keyword"" },
                            ""service_name"": { ""type"": ""keyword"" },
                            ""level"": { ""type"": ""keyword"" },
                            ""thread_name"": { ""type"": ""keyword"" },
                            ""logger_name"": { ""type"": ""keyword"" },
                            ""host_ip"": { ""type"": ""ip"" },
                            ""message"": { ""type"": ""text"" },
                            ""exception"": {
                                ""type"": ""object"",
                                ""properties"": {
                                    ""type"": { ""type"": ""keyword"" },
                                    ""message"": { ""type"": ""text"" },
                                    ""stack_trace"": { ""type"": ""text"", ""index"": false },
                                    ""inner_exception"": {
                                        ""type"": ""object"",
                                        ""properties"": {
                                            ""type"": { ""type"": ""keyword"" },
                                            ""message"": { ""type"": ""text"" },
                                            ""stack_trace"": { ""type"": ""text"", ""index"": false }
                                        }
                                    }
                                }
                            },
                            ""http_status_code"": { ""type"": ""integer"" },
                            ""response_time_ms"": { ""type"": ""long"" }
                        }
                    }
                }
            }";

            var response = await _client.LowLevel.DoRequestAsync<StringResponse>(
                Elasticsearch.Net.HttpMethod.PUT,
                "/_index_template/application_logs_template",
                CancellationToken.None,
                PostData.String(templateJson)
            );

            if (response.Success)
            {
                _logger.LogInformation("application_logs modern index template created successfully");
                return true;
            }
            else
            {
                _logger.LogError("Modern index template creation error: {Error}", response.Body);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating template");
            return false;
        }
    }

    public async Task<bool> CreateProductsIndexAsync()
    {
        try
        {
            var indexExists = await _client.Indices.ExistsAsync("products");
            if (indexExists.Exists)
            {
                _logger.LogInformation("products index already exists");
                return true;
            }

            var createIndexResponse = await _client.Indices.CreateAsync("products", c => c
                .Map<Product>(m => m.AutoMap())
            );

            if (createIndexResponse.IsValid)
            {
                _logger.LogInformation("products index created successfully");
                return true;
            }
            else
            {
                _logger.LogError("products index creation error: {Error}", createIndexResponse.DebugInformation);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating products index");
            return false;
        }
    }

    public async Task<bool> LoadSampleProductsAsync()
    {
        try
        {
            var sampleProducts = GetSampleProducts();

            var bulkResponse = await _client.BulkAsync(b => b
                .Index("products")
                .IndexMany(sampleProducts, (bd, product) => bd.Id(product.Sku).Document(product))
            );

            // NEST sometimes incorrectly parses successful bulk operations
            // So we check both NEST validation and HTTP status
            var httpSuccessful = bulkResponse.ApiCall?.HttpStatusCode >= 200 && bulkResponse.ApiCall?.HttpStatusCode < 300;
            var hasErrors = bulkResponse.Errors;

            if (httpSuccessful && !hasErrors)
            {
                var successCount = bulkResponse.Items?.Count(item =>
                    item.Status >= 200 && item.Status < 300) ?? 0;

                _logger.LogInformation("{Count} sample products loaded successfully (HTTP: {HttpStatus})",
                    successCount, bulkResponse.ApiCall?.HttpStatusCode);
                return true;
            }
            else if (httpSuccessful && hasErrors)
            {
                // HTTP successful but NEST errors flag is true - detailed check
                var successItems = bulkResponse.Items?.Where(item =>
                    item.Status >= 200 && item.Status < 300).ToList() ?? new();
                var errorItems = bulkResponse.Items?.Where(item =>
                    item.Status < 200 || item.Status >= 300).ToList() ?? new();

                if (successItems.Any() && !errorItems.Any())
                {
                    // Actually all successful, NEST is giving false positive
                    _logger.LogInformation("{Count} sample products loaded successfully (NEST false positive error ignored)",
                        successItems.Count);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Partial success: {SuccessCount} successful, {ErrorCount} failed",
                        successItems.Count, errorItems.Count);

                    foreach (var errorItem in errorItems)
                    {
                        _logger.LogError("Failed item: {Id}, Status: {Status}, Error: {Error}",
                            errorItem.Id, errorItem.Status, errorItem.Error?.Reason);
                    }

                    return successItems.Count > 0; // Returns true if at least some succeeded
                }
            }
            else
            {
                _logger.LogError("Sample product loading error: HTTP {HttpStatus}, Errors: {HasErrors}, Debug: {Debug}",
                    bulkResponse.ApiCall?.HttpStatusCode, hasErrors, bulkResponse.DebugInformation);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while loading sample products");
            return false;
        }
    }

    private List<Product> GetSampleProducts()
    {
        return new List<Product>
        {
            new()
            {
                Sku = "SKU001",
                Name = "Awesome Laptop Model Z",
                Description = "A powerful laptop with the latest features.",
                Price = 1399.99m,
                StockQuantity = 15,
                Category = "Computers",
                Tags = new List<string> { "laptop", "new-gen", "gaming" },
                CreatedDate = DateTime.Parse("2024-05-20T10:00:00Z"),
                IsActive = true
            },
            new()
            {
                Sku = "SKU002",
                Name = "Super Silent Mechanical Keyboard",
                Description = "RGB backlit, long-lasting mechanical keyboard.",
                Price = 75.50m,
                StockQuantity = 45,
                Category = "Accessories",
                Tags = new List<string> { "keyboard", "mechanical", "rgb" },
                CreatedDate = DateTime.Parse("2024-05-21T14:30:00Z"),
                IsActive = true
            },
            new()
            {
                Sku = "SKU003",
                Name = "Wide Screen Monitor",
                Description = "Ideal for professional use, 32 inch 4K monitor.",
                Price = 450.00m,
                StockQuantity = 0,
                Category = "Monitors",
                Tags = new List<string> { "monitor", "4k", "professional" },
                CreatedDate = DateTime.Parse("2024-05-15T09:15:00Z"),
                IsActive = false
            },
            new()
            {
                Sku = "SKU004",
                Name = "Ergonomic Office Chair",
                Description = "Comfortable ergonomic chair for long working hours.",
                Price = 299.00m,
                StockQuantity = 25,
                Category = "Furniture",
                Tags = new List<string> { "chair", "ergonomic", "office" },
                CreatedDate = DateTime.Parse("2024-05-22T08:45:00Z"),
                IsActive = true
            },
            new()
            {
                Sku = "SKU005",
                Name = "Wireless Bluetooth Headphones",
                Description = "High-quality noise-cancelling wireless headphones.",
                Price = 199.99m,
                StockQuantity = 50,
                Category = "Audio",
                Tags = new List<string> { "headphones", "wireless", "bluetooth", "noise-cancelling" },
                CreatedDate = DateTime.Parse("2024-05-23T16:20:00Z"),
                IsActive = true
            }
        };
    }
}

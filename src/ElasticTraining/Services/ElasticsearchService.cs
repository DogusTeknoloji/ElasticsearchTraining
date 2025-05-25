using ElasticTraining.Models;
using Nest;

namespace ElasticTraining.Services;

public class ElasticsearchService : IElasticsearchService
{
    private readonly IElasticClient _client;
    private readonly ILogger<ElasticsearchService> _logger;

    public ElasticsearchService(IElasticClient client, ILogger<ElasticsearchService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<bool> IsElasticsearchAvailableAsync()
    {
        try
        {
            var response = await _client.PingAsync();
            return response.IsValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Elasticsearch bağlantı kontrolü sırasında hata oluştu");
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
            return "Cluster health bilgisi alınamadı";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cluster health bilgisi alınırken hata oluştu");
            return "Hata: " + ex.Message;
        }
    }

    public async Task<bool> CreateApplicationLogsTemplateAsync()
    {
        try
        {
            var templateRequest = new PutIndexTemplateRequest("application_logs_template")
            {
                IndexPatterns = new[] { "application_logs-*" },
                Mappings = new TypeMapping
                {
                    Dynamic = false,
                    Properties = new Properties
                    {
                        ["@timestamp"] = new DateProperty(),
                        ["correlation_id"] = new KeywordProperty(),
                        ["service_name"] = new KeywordProperty(),
                        ["level"] = new KeywordProperty(),
                        ["thread_name"] = new KeywordProperty(),
                        ["logger_name"] = new KeywordProperty(),
                        ["host_ip"] = new IpProperty(),
                        ["message"] = new TextProperty(),
                        ["exception"] = new ObjectProperty
                        {
                            Properties = new Properties
                            {
                                ["type"] = new KeywordProperty(),
                                ["message"] = new TextProperty(),
                                ["stack_trace"] = new TextProperty { Index = false },
                                ["inner_exception"] = new ObjectProperty
                                {
                                    Properties = new Properties
                                    {
                                        ["type"] = new KeywordProperty(),
                                        ["message"] = new TextProperty(),
                                        ["stack_trace"] = new TextProperty { Index = false }
                                    }
                                }
                            }
                        },
                        ["http_status_code"] = new NumberProperty(NumberType.Integer),
                        ["response_time_ms"] = new NumberProperty(NumberType.Long)
                    }
                }
            };

            var response = await _client.Indices.PutTemplateAsync(templateRequest);
            
            if (response.IsValid)
            {
                _logger.LogInformation("application_logs template başarıyla oluşturuldu");
                return true;
            }
            else
            {
                _logger.LogError("Template oluşturma hatası: {Error}", response.DebugInformation);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Template oluşturulurken hata oluştu");
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
                _logger.LogInformation("products index zaten mevcut");
                return true;
            }

            var createIndexResponse = await _client.Indices.CreateAsync("products", c => c
                .Map<Product>(m => m.AutoMap())
            );

            if (createIndexResponse.IsValid)
            {
                _logger.LogInformation("products index başarıyla oluşturuldu");
                return true;
            }
            else
            {
                _logger.LogError("products index oluşturma hatası: {Error}", createIndexResponse.DebugInformation);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "products index oluşturulurken hata oluştu");
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

            if (bulkResponse.IsValid)
            {
                _logger.LogInformation("{Count} örnek ürün başarıyla yüklendi", sampleProducts.Count);
                return true;
            }
            else
            {
                _logger.LogError("Örnek ürün yükleme hatası: {Error}", bulkResponse.DebugInformation);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Örnek ürünler yüklenirken hata oluştu");
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

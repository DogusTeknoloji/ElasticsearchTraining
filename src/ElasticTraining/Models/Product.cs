using Nest;

namespace ElasticTraining.Models;

[ElasticsearchType(IdProperty = nameof(Sku))]
public class Product
{
    [Keyword]
    public string Sku { get; set; } = string.Empty;

    [Text(Analyzer = "english", Name = "product_name")]
    public string Name { get; set; } = string.Empty;

    [Text(Analyzer = "english", Name = "product_description")]
    public string Description { get; set; } = string.Empty;

    [Number(NumberType.ScaledFloat, ScalingFactor = 100)]
    public decimal Price { get; set; }

    [Number(NumberType.Integer, Name = "stock_quantity")]
    public int StockQuantity { get; set; }

    [Keyword]
    public string Category { get; set; } = string.Empty;

    [Keyword]
    public List<string> Tags { get; set; } = new();

    [Date(Name = "created_date")]
    public DateTime CreatedDate { get; set; }

    [Boolean(Name = "is_active")]
    public bool IsActive { get; set; }
}

using Nest;

namespace ElasticTraining.Models;

[ElasticsearchType(IdProperty = nameof(Sku))]
public class Product
{
    [Keyword]
    public string Sku { get; set; } = string.Empty;

    [Text(Analyzer = "english")]
    [Keyword(Name = "keyword")]
    public string Name { get; set; } = string.Empty;

    [Text(Analyzer = "english")]
    public string Description { get; set; } = string.Empty;

    [Number(NumberType.ScaledFloat, ScalingFactor = 100)]
    public decimal Price { get; set; }

    [Number(NumberType.Integer)]
    public int StockQuantity { get; set; }

    [Keyword]
    public string Category { get; set; } = string.Empty;

    [Keyword]
    public List<string> Tags { get; set; } = new();

    [Date]
    public DateTime CreatedDate { get; set; }

    [Boolean]
    public bool IsActive { get; set; }
}

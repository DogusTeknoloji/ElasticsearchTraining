namespace ElasticTraining.Configuration;

public class ElasticsearchConfiguration
{
    public string Uri { get; set; } = "http://localhost:9200";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

using ElasticTraining.Configuration;
using ElasticTraining.Services;
using Nest;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Elasticsearch Configuration
var elasticsearchConfig =
    builder.Configuration.GetSection("Elasticsearch").Get<ElasticsearchConfiguration>()
    ?? new ElasticsearchConfiguration();

builder.Services.AddSingleton(elasticsearchConfig);

// Elasticsearch Client
builder.Services.AddSingleton<IElasticClient>(provider =>
{
    var config = provider.GetRequiredService<ElasticsearchConfiguration>();
    var settings = new ConnectionSettings(new Uri(config.Uri))
        .DefaultIndex("application_logs")
        .DisableDirectStreaming(); // Debug için yararlı

    if (!string.IsNullOrEmpty(config.Username) && !string.IsNullOrEmpty(config.Password))
    {
        settings.BasicAuthentication(config.Username, config.Password);
    }

    return new ElasticClient(settings);
});

// Application Services
builder.Services.AddScoped<IElasticsearchService, ElasticsearchService>();
builder.Services.AddSingleton<ILogGeneratorService, LogGeneratorService>();
builder.Services.AddHostedService<LogGeneratorService>(provider => 
    (LogGeneratorService)provider.GetRequiredService<ILogGeneratorService>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

await app.RunAsync();

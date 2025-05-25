using ElasticTraining.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ElasticTraining.Pages;

public class IndexModel(
    ILogger<IndexModel> logger,
    IElasticsearchService elasticsearchService,
    ILogGeneratorService logGeneratorService)
    : PageModel
{
    private readonly ILogger<IndexModel> _logger = logger;
    private readonly IElasticsearchService _elasticsearchService = elasticsearchService;
    private readonly ILogGeneratorService _logGeneratorService = logGeneratorService;

    [BindProperty]
    public string? Message { get; set; }

    [BindProperty]
    public string? ClusterHealth { get; set; }

    [BindProperty]
    public bool IsElasticsearchAvailable { get; set; }

    [BindProperty]
    public bool IsLogGeneratorRunning { get; set; }

    [BindProperty]
    public bool IsGeneratingHistoricalData { get; set; }

    public async Task OnGetAsync()
    {
        await CheckElasticsearchStatus();
        IsLogGeneratorRunning = _logGeneratorService.IsRunning;
        IsGeneratingHistoricalData = _logGeneratorService.IsGeneratingHistoricalData;
    }

    public async Task<IActionResult> OnPostCreateTemplateAsync()
    {
        var success = await _elasticsearchService.CreateApplicationLogsTemplateAsync();
        Message = success ? "✅ application_logs template created successfully!" : "❌ Template creation failed!";

        await CheckElasticsearchStatus();
        IsLogGeneratorRunning = _logGeneratorService.IsRunning;
        IsGeneratingHistoricalData = _logGeneratorService.IsGeneratingHistoricalData;
        return Page();
    }

    public async Task<IActionResult> OnPostCreateProductsIndexAsync()
    {
        var success = await _elasticsearchService.CreateProductsIndexAsync();
        Message = success ? "✅ products index created successfully!" : "❌ Index creation failed!";

        await CheckElasticsearchStatus();
        IsLogGeneratorRunning = _logGeneratorService.IsRunning;
        IsGeneratingHistoricalData = _logGeneratorService.IsGeneratingHistoricalData;
        return Page();
    }
    public async Task<IActionResult> OnPostLoadSampleProductsAsync()
    {
        var success = await _elasticsearchService.LoadSampleProductsAsync();
        Message = success ? "✅ Sample products loaded successfully!" : "❌ Product loading failed!";

        await CheckElasticsearchStatus();
        IsLogGeneratorRunning = _logGeneratorService.IsRunning;
        IsGeneratingHistoricalData = _logGeneratorService.IsGeneratingHistoricalData;
        return Page();
    }

    public async Task<IActionResult> OnPostStartLogGeneratorAsync()
    {
        await _logGeneratorService.StartAsync();
        Message = "✅ Log generation started!";

        await CheckElasticsearchStatus();
        IsLogGeneratorRunning = _logGeneratorService.IsRunning;
        IsGeneratingHistoricalData = _logGeneratorService.IsGeneratingHistoricalData;
        return Page();
    }

    public async Task<IActionResult> OnPostStopLogGeneratorAsync()
    {
        await _logGeneratorService.StopAsync();
        Message = "⏹️ Log generation stopped!";

        await CheckElasticsearchStatus();
        IsLogGeneratorRunning = _logGeneratorService.IsRunning;
        IsGeneratingHistoricalData = _logGeneratorService.IsGeneratingHistoricalData;
        return Page();
    }

    public async Task<IActionResult> OnPostGenerateHistoricalDataAsync()
    {
        if (_logGeneratorService.IsGeneratingHistoricalData)
        {
            Message = "⚠️ Historical data generation is already in progress!";
        }
        else
        {
            // Start historical data generation in background
            _ = Task.Run(async () => await _logGeneratorService.GenerateHistoricalDataAsync(30));
            Message = "🕐 Historical data generation started! This will generate 30 days of log data in the background.";
        }

        await CheckElasticsearchStatus();
        IsLogGeneratorRunning = _logGeneratorService.IsRunning;
        IsGeneratingHistoricalData = _logGeneratorService.IsGeneratingHistoricalData;
        return Page();
    }

    private async Task CheckElasticsearchStatus()
    {
        IsElasticsearchAvailable = await _elasticsearchService.IsElasticsearchAvailableAsync();
        ClusterHealth = await _elasticsearchService.GetClusterHealthAsync();
    }
}

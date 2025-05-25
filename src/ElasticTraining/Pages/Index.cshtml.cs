using ElasticTraining.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ElasticTraining.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IElasticsearchService _elasticsearchService;
    private readonly ILogGeneratorService _logGeneratorService;

    public IndexModel(ILogger<IndexModel> logger, IElasticsearchService elasticsearchService, ILogGeneratorService logGeneratorService)
    {
        _logger = logger;
        _elasticsearchService = elasticsearchService;
        _logGeneratorService = logGeneratorService;
    }

    [BindProperty]
    public string? Message { get; set; }

    [BindProperty]
    public string? ClusterHealth { get; set; }

    [BindProperty]
    public bool IsElasticsearchAvailable { get; set; }

    [BindProperty]
    public bool IsLogGeneratorRunning { get; set; }

    public async Task OnGetAsync()
    {
        await CheckElasticsearchStatus();
        IsLogGeneratorRunning = _logGeneratorService.IsRunning;
    }

    public async Task<IActionResult> OnPostCreateTemplateAsync()
    {
        var success = await _elasticsearchService.CreateApplicationLogsTemplateAsync();
        Message = success ? "✅ application_logs template başarıyla oluşturuldu!" : "❌ Template oluşturma başarısız!";
        
        await CheckElasticsearchStatus();
        IsLogGeneratorRunning = _logGeneratorService.IsRunning;
        return Page();
    }

    public async Task<IActionResult> OnPostCreateProductsIndexAsync()
    {
        var success = await _elasticsearchService.CreateProductsIndexAsync();
        Message = success ? "✅ products index başarıyla oluşturuldu!" : "❌ Index oluşturma başarısız!";
        
        await CheckElasticsearchStatus();
        IsLogGeneratorRunning = _logGeneratorService.IsRunning;
        return Page();
    }

    public async Task<IActionResult> OnPostLoadSampleProductsAsync()
    {
        var success = await _elasticsearchService.LoadSampleProductsAsync();
        Message = success ? "✅ Örnek ürünler başarıyla yüklendi!" : "❌ Ürün yükleme başarısız!";
        
        await CheckElasticsearchStatus();
        IsLogGeneratorRunning = _logGeneratorService.IsRunning;
        return Page();
    }

    public async Task<IActionResult> OnPostStartLogGeneratorAsync()
    {
        await _logGeneratorService.StartAsync();
        Message = "✅ Log üretimi başlatıldı!";
        
        await CheckElasticsearchStatus();
        IsLogGeneratorRunning = _logGeneratorService.IsRunning;
        return Page();
    }

    public async Task<IActionResult> OnPostStopLogGeneratorAsync()
    {
        await _logGeneratorService.StopAsync();
        Message = "⏹️ Log üretimi durduruldu!";
        
        await CheckElasticsearchStatus();
        IsLogGeneratorRunning = _logGeneratorService.IsRunning;
        return Page();
    }

    private async Task CheckElasticsearchStatus()
    {
        IsElasticsearchAvailable = await _elasticsearchService.IsElasticsearchAvailableAsync();
        ClusterHealth = await _elasticsearchService.GetClusterHealthAsync();
    }
}

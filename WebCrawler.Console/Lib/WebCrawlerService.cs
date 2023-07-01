using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebCrawler.Console.Config;

namespace WebCrawler.Console.Lib;

public class WebCrawlerService : IWebCrawlerService
{
    private readonly ILogger<WebCrawlerService> _logger;
    private readonly bool _crawlOnlyHost;
    public WebCrawlerService(ILogger<WebCrawlerService> logger, IOptions<CrawlerOptions> config)
    {
        _logger = logger;
        _crawlOnlyHost = config.Value.CrawlOnlyHost;
    }
    
    public async Task RunAsync(string urlTarget)
    {
        _logger.LogInformation($"Starting Crawler at {urlTarget}.");
        _logger.LogInformation($"Crawling only designated host: {_crawlOnlyHost}");
        await Task.Delay(10);
        
        _logger.LogInformation("Crawler Complete.");
        return;
    }
}
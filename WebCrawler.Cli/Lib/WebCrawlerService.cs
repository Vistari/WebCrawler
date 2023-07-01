using Microsoft.Extensions.Options;
using WebCrawler.Cli.Config;
using Console = System.Console;

namespace WebCrawler.Cli.Lib;

public class WebCrawlerService : IWebCrawlerService
{
    private readonly bool _crawlOnlyHost;
    public WebCrawlerService(IOptions<CrawlerOptions> config)
    {
        _crawlOnlyHost = config.Value.CrawlOnlyHost;
    }
    
    public async Task RunAsync(string urlTarget)
    {
        Console.WriteLine($"Starting Crawler at {urlTarget}.");
        Console.WriteLine($"Crawling only designated host: {_crawlOnlyHost}");
        await Task.Delay(10);
        
        Console.WriteLine("Crawler Complete.");
        return;
    }
}
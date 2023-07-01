using Microsoft.Extensions.Options;
using WebCrawler.Cli.Config;
using Console = System.Console;

namespace WebCrawler.Cli.Lib;

public class WebCrawlerService : IWebCrawlerService
{
    public WebCrawlerService()
    {
    }
    
    public async Task RunAsync(string urlTarget)
    {
        Console.WriteLine($"Starting Crawler at {urlTarget}.");
        await Task.Delay(10);
        
        Console.WriteLine("Crawler Complete.");
        return;
    }
}
using System.Collections.Concurrent;

namespace WebCrawler.Cli.Lib;

public interface IWebCrawlerService
{
    Task<ConcurrentDictionary<string, List<string>>> RunAsync(string urlTarget);
}
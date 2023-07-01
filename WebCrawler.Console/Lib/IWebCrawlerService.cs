using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebCrawler.Console.Lib;

public interface IWebCrawlerService
{
    Task RunAsync(string urlTarget);
}
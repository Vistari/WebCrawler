namespace WebCrawler.Cli.Lib;

public interface IWebCrawlerService
{
    Task RunAsync(string urlTarget);
}
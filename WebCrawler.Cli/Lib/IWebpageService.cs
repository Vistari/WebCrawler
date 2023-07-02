namespace WebCrawler.Cli.Lib;

public interface IWebpageService
{
    Task<string?> GetWebpage(Uri uri);
}
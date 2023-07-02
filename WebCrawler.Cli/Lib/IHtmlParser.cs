namespace WebCrawler.Cli.Lib;

public interface IHtmlParser
{
    Task<IEnumerable<string>> ParseLinks(string html);
}
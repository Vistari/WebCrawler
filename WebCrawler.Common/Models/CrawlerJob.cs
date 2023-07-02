namespace WebCrawler.Common.Models;

public record CrawlerJob(string Url)
{
    private Uri? _Uri;
    
    public Uri Uri => _Uri ??= new Uri(Url);
};
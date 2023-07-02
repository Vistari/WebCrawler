namespace WebCrawler.Common.Models;

public record ProcessedLink
{
    public Uri Uri { get; }
    
    public string OriginalUrl { get; }
    
    public bool IsExternalUrl { get; }
};
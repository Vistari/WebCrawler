using WebCrawler.Cli.Lib;

namespace WebCrawler.Cli.Tests.Lib;

public class HtmlParserTests
{
    private readonly HtmlParser _parser;

    public HtmlParserTests()
    {
        _parser = new HtmlParser();
    }

    [Fact]
    public async Task HtmlParser_ParseLinks_NullParamReturnsEmptyList()
    {
        var result = await _parser.ParseLinks(null);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task HtmlParser_ParseLinks_EmptyParamReturnsEmptyList()
    {
        var result = await _parser.ParseLinks(string.Empty);
        Assert.Empty(result);
    }

    [Fact]
    public async Task HtmlParser_ParseLinks_NoATagsReturnsEmptyList()
    {
        const string html = "<html><body><p>Test</p></body></html>";
        
        var result = await _parser.ParseLinks(html);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task HtmlParser_ParseLinks_NoTagsButHrefsReturnsEmptyList()
    {
        const string html = "<html><body><p href=\"test\">Test</p></body></html>";
        
        var result = await _parser.ParseLinks(html);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task HtmlParser_ParseLinks_ATagsNoHrefReturnsEmptyList()
    {
        const string html = "<html><body><a>Test</a></body></html>";
        
        var result = await _parser.ParseLinks(html);
        Assert.Empty(result);
    }

    [Fact]
    public async Task HtmlParser_ParseLinks_ATagsWithHrefReturnsListOfLinks()
    {
        var links = new List<string>()
        {
            "<a href=\"test\">test</a>",
            "<a href=\"test2\">test2</a>",
        };
        
        var html = $"<html><body>" + string.Join("", links) + "</body></html>";
        
        var result = await _parser.ParseLinks(html);
        Assert.Equal(links.Count, result.Count());
    }
}
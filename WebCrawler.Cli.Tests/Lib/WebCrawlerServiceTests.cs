using Moq;
using WebCrawler.Cli.Lib;

namespace WebCrawler.Cli.Tests.Lib;

public class WebCrawlerServiceTests
{
    private readonly Mock<IHtmlParser> _mockHtmlParser;
    private readonly Mock<IWebpageService> _mockWebpageService;
    private readonly WebCrawlerService _webCrawlerService;
    
    public WebCrawlerServiceTests()
    {
        _mockHtmlParser = new Mock<IHtmlParser>();
        _mockWebpageService = new Mock<IWebpageService>();
        _webCrawlerService = new WebCrawlerService(_mockWebpageService.Object, _mockHtmlParser.Object);
    }
    
    [Fact]
    public async Task WebCrawlerServiceTests_RunAsync_NullParamThrows()
    {
        await Assert.ThrowsAsync<ArgumentException>(async () => await _webCrawlerService.RunAsync(null));
    }
    
    [Fact]
    public async Task WebCrawlerServiceTests_RunAsync_EmptyParamThrows()
    {
        await Assert.ThrowsAsync<ArgumentException>(async () => await _webCrawlerService.RunAsync(string.Empty));
    }
    
    [Fact]
    public async Task WebCrawlerServiceTests_RunAsync_NotValidUriThrows()
    {
        await Assert.ThrowsAsync<UriFormatException>(async () => await _webCrawlerService.RunAsync("test"));
    }
    
    [Fact]
    public async Task WebCrawlerServiceTests_RunAsync_WebpageNoLinksReturnsSingleResult()
    {
        _mockWebpageService.Setup(x => x.GetWebpage(It.IsAny<Uri>())).ReturnsAsync(() => "<html></html>");
        
        var result = await _webCrawlerService.RunAsync("https://www.test.com");
        
        Assert.True(result.Count == 1);
        Assert.Empty(result.Single().Value);
    }
    
    [Fact]
    public async Task WebCrawlerServiceTests_RunAsync_WebpageNullLinkIgnored()
    {
        _mockWebpageService.Setup(x => x.GetWebpage(It.IsAny<Uri>())).ReturnsAsync(() => null);
        
        var result = await _webCrawlerService.RunAsync("https://www.test.com");
        
        Assert.True(result.Count == 1);
        Assert.Empty(result.Single().Value);
    }
    
    [Fact]
    public async Task WebCrawlerServiceTests_RunAsync_WebpageEmptyLinkAddedButNotProcessed()
    {
        _mockWebpageService.Setup(x => x.GetWebpage(It.IsAny<Uri>())).ReturnsAsync(() => $"<html><a href=\"\"></a></html>");
        _mockHtmlParser.Setup(x => x.ParseLinks(It.IsAny<string>())).ReturnsAsync(() => new List<string>() { string.Empty});
        var result = await _webCrawlerService.RunAsync("https://www.test.com");
        
        Assert.True(result.Count == 1);
        Assert.True(result.Single().Value.Single() == string.Empty);
    }
    
    [Theory]
    [InlineData("mailto:test@test.com")]
    [InlineData("sms:0123456789012")]
    [InlineData("tel:0123456789012")]
    [InlineData("#someId")]
    [InlineData("javascript:DoAThing")]
    [InlineData("monzo:some-ref")]
    public async Task WebCrawlerServiceTests_RunAsync_WebpageInvalidQualifierLinkAddedButNotProcessed(string invalidQualifierLink)
    {
        _mockWebpageService.Setup(x => x.GetWebpage(It.IsAny<Uri>())).ReturnsAsync(() => $"<html><a href=\"{invalidQualifierLink}\"></a></html>");
        _mockHtmlParser.Setup(x => x.ParseLinks(It.IsAny<string>())).ReturnsAsync(() => new List<string>() { invalidQualifierLink });
        var result = await _webCrawlerService.RunAsync("https://www.test.com");
        
        Assert.True(result.Count == 1);
        Assert.True(result.Single().Value.Single() == invalidQualifierLink);
    }
    
    [Theory]
    [InlineData("https://www.test.com/other-link")]
    [InlineData("http://www.test.com/other-other-link")]
    public async Task WebCrawlerServiceTests_RunAsync_WebpageFullyQualifiedLinkIsCrawled(string fullyQualifiedLink)
    {
        var initialUrl = "https://www.test.com";
        _mockWebpageService.SetupSequence(x => x.GetWebpage(It.IsAny<Uri>()))
            .ReturnsAsync(() => $"<html><a href=\"{fullyQualifiedLink}\"></a></html>")
            .ReturnsAsync(() => $"<html></html>");
        _mockHtmlParser.SetupSequence(x => x.ParseLinks(It.IsAny<string>()))
            .ReturnsAsync(() => new List<string>() { fullyQualifiedLink })
            .ReturnsAsync(() => new List<string>());
        var result = await _webCrawlerService.RunAsync(initialUrl);
        
        Assert.True(result.Count == 2);
        Assert.True(result.ContainsKey(initialUrl));
        Assert.True(result.ContainsKey(fullyQualifiedLink));
        Assert.True(result[initialUrl].Single() == fullyQualifiedLink);
    }
    
    [Theory]
    [InlineData("www.test.com/other-link", "https://www.test.com/other-link")]
    [InlineData("/other-link", "https://www.test.com/other-link")]
    [InlineData("other-link", "https://www.test.com/other-link")]
    public async Task WebCrawlerServiceTests_RunAsync_WebpageNotFullyQualifiedLinkIsCrawled(string notFullyQualifiedLink, string convertedLink)
    {
        var initialUrl = "https://www.test.com";
        _mockWebpageService.SetupSequence(x => x.GetWebpage(It.IsAny<Uri>()))
            .ReturnsAsync(() => $"<html><a href=\"{notFullyQualifiedLink}\"></a></html>")
            .ReturnsAsync(() => $"<html></html>");
        _mockHtmlParser.SetupSequence(x => x.ParseLinks(It.IsAny<string>()))
            .ReturnsAsync(() => new List<string>() { notFullyQualifiedLink })
            .ReturnsAsync(() => new List<string>());
        var result = await _webCrawlerService.RunAsync(initialUrl);
        
        Assert.True(result.Count == 2);
        Assert.True(result.ContainsKey(initialUrl));
        Assert.True(result.ContainsKey(convertedLink));
        Assert.True(result[initialUrl].Single() == notFullyQualifiedLink);
    }
    
    [Fact]
    public async Task WebCrawlerServiceTests_RunAsync_WebpageDifferentHostIgnored()
    {
        var differentHostLink = "https://www.some-other-host.com";
        _mockWebpageService.Setup(x => x.GetWebpage(It.IsAny<Uri>())).ReturnsAsync(() => $"<html><a href=\"{differentHostLink}\"></a></html>");
        _mockHtmlParser.Setup(x => x.ParseLinks(It.IsAny<string>())).ReturnsAsync(() => new List<string>() { differentHostLink });
        var result = await _webCrawlerService.RunAsync("https://www.test.com");
        
        Assert.True(result.Count == 1);
        Assert.Empty(result.Single().Value);
    }

    [Fact]
    public async Task WebCrawlerServiceTests_RunAsync_WebpageDuplicateLinksOnlyInResultsOnce()
    {
        var initialLink = "https://www.test.com";
        var duplicateLink = "https://www.test.com/other-url";
        _mockWebpageService.Setup(x => x.GetWebpage(It.IsAny<Uri>())).ReturnsAsync(() => $"<html><a href=\"{duplicateLink}\"></a><a href=\"{duplicateLink}\"></a></html>");
        _mockHtmlParser.Setup(x => x.ParseLinks(It.IsAny<string>())).ReturnsAsync(() => new List<string>() { duplicateLink });
        var result = await _webCrawlerService.RunAsync(initialLink);
        
        Assert.True(result.Count == 2);
        Assert.True(result.ContainsKey(initialLink));
        Assert.True(result.ContainsKey(duplicateLink));
        Assert.Equal(1,result[initialLink].Count(x=> x.Equals(duplicateLink)));
    }
    
    [Fact]
    public async Task WebCrawlerServiceTests_RunAsync_WebpageDuplicateLinksOnlyCrawledOnce()
    {
        var initialLink = "https://www.test.com";
        var duplicateLink = "https://www.test.com";
        _mockWebpageService.Setup(x => x.GetWebpage(It.IsAny<Uri>())).ReturnsAsync(() => $"<html><a href=\"{duplicateLink}\"></a></html>");
        _mockHtmlParser.Setup(x => x.ParseLinks(It.IsAny<string>())).ReturnsAsync(() => new List<string>() { duplicateLink });
        var result = await _webCrawlerService.RunAsync(initialLink);
        
        Assert.True(result.Count == 2);
        Assert.True(result.ContainsKey(initialLink));
        Assert.True(result.ContainsKey(duplicateLink));
    }
}
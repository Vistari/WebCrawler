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
        
    }
    
    [Fact]
    public async Task WebCrawlerServiceTests_RunAsync_EmptyParamThrows()
    {
        
    }
    
    [Fact]
    public async Task WebCrawlerServiceTests_RunAsync_WebpageNoLinks()
    {
        
    }
}
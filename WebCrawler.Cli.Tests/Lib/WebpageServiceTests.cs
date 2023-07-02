using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using Moq;
using Moq.Protected;
using WebCrawler.Cli.Lib;

namespace WebCrawler.Cli.Tests.Lib;

public class WebpageServiceTests
{
    private readonly WebpageService _webpageService;
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;

    public WebpageServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _webpageService = new WebpageService(new HttpClient(_mockHttpMessageHandler.Object));
    }

    [Fact]
    public async Task WebpageService_GetWebpage_UnexpectedSchemeReturnsNull()
    {
        var result = await _webpageService.GetWebpage(new UriBuilder()
        {
            Scheme = "test://",
            Host = "www.mywebsite.com",
            Path = "/test"
        }.Uri);

        Assert.Null(result);
    }

    [Fact]
    public async Task WebpageService_GetWebpage_NotUsableMediaTypeReturnsNull()
    {
        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new FormUrlEncodedContent(new Dictionary<string, string>())
            });

        var result = await _webpageService.GetWebpage(new UriBuilder()
        {
            Scheme = "https://",
            Host = "www.mywebsite.com",
            Path = "/test"
        }.Uri);

        Assert.Null(result);
    }

    [Fact]
    public async Task WebpageService_GetWebpage_UsableMediaTypeReturnsResponseContent()
    {
        var expectedStringContent = "<html><body><p href=\"test\">Test</p></body></html>";
        
        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expectedStringContent, new MediaTypeHeaderValue(MediaTypeNames.Text.Html))
            });

        var result = await _webpageService.GetWebpage(new UriBuilder()
        {
            Scheme = "https://",
            Host = "www.mywebsite.com",
            Path = "/test"
        }.Uri);

        Assert.Equal(expectedStringContent,result);
        
    }
}
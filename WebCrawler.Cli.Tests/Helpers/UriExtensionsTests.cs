using WebCrawler.Cli.Helpers;

namespace WebCrawler.Cli.Tests.Helpers;

public class UriExtensionsTests
{
    [Theory]
    [InlineData("https://www.my-website.com", "my/extra/path", "https://www.my-website.com/my/extra/path")]
    [InlineData("https://www.my-website.com", "/my/extra/path", "https://www.my-website.com/my/extra/path")]
    [InlineData("https://www.my-website.com", "//my/extra/path", "https://www.my-website.com/my/extra/path")]
    [InlineData("https://www.my-website.com/", "my/extra/path", "https://www.my-website.com/my/extra/path")]
    [InlineData("https://www.my-website.com/", "/my/extra/path", "https://www.my-website.com/my/extra/path")]
    [InlineData("https://www.my-website.com/", "//my/extra/path", "https://www.my-website.com/my/extra/path")]
    [InlineData("https://www.my-website.com/with/some/path", "my/extra/path", "https://www.my-website.com/with/some/path/my/extra/path")]
    [InlineData("https://www.my-website.com/with/some/path", "/my/extra/path", "https://www.my-website.com/with/some/path/my/extra/path")]
    [InlineData("https://www.my-website.com/with/some/path", "//my/extra/path", "https://www.my-website.com/with/some/path/my/extra/path")]
    [InlineData("https://www.my-website.com/with/some/path/", "my/extra/path", "https://www.my-website.com/with/some/path/my/extra/path")]
    [InlineData("https://www.my-website.com/with/some/path/", "/my/extra/path", "https://www.my-website.com/with/some/path/my/extra/path")]
    [InlineData("https://www.my-website.com/with/some/path/", "//my/extra/path", "https://www.my-website.com/with/some/path/my/extra/path")]
    public void UriExtensions_UriCorrectlyCombined(string baseUrl, string path, string expectedResult)
    {
        var uri = new Uri(baseUrl);

        var result = uri.Append(path);
        
        Assert.Equal(expectedResult, result.ToString());
    }
}
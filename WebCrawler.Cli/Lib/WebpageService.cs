using System.Net;
using System.Net.Mime;

namespace WebCrawler.Cli.Lib;

public class WebpageService : IWebpageService
{
    private readonly HttpClient _httpClient;
    private const int MaxRetries = 3;

    public WebpageService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> GetWebpage(Uri uri)
    {
        if (!uri.Scheme.ToLower().Equals("http") && !uri.Scheme.ToLower().Equals("https"))
        {
            return null;
        }

        var result = await GetPage(uri);

        if (result is not { IsSuccessStatusCode: true })
        {
            Console.WriteLine($"Unable to read {uri}. Received HTTP {result?.StatusCode}");
        }
        
        //Not a content type we want to parse, return null
        if (result?.Content.Headers.ContentType?.MediaType != MediaTypeNames.Text.Html)
        {
            return null;
        }

        return await result.Content.ReadAsStringAsync();
    }

    private async Task<HttpResponseMessage?> GetPage(Uri uri)
    {
        var tryCount = 0;

        HttpResponseMessage? response = null;
        
        while (tryCount < MaxRetries)
        {
            tryCount += 1;

            response = await _httpClient.GetAsync(uri);

            //If the request timed out and we still have retries left, try again after a few seconds
            if (response.StatusCode.Equals(HttpStatusCode.RequestTimeout) ||
                 response.StatusCode.Equals(HttpStatusCode.GatewayTimeout))
            {
                Thread.Sleep(5000); ;
                continue;
            }

            break;
        }

        return response;
    }
}
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using WebCrawler.Cli.Config;
using WebCrawler.Cli.Helpers;
using WebCrawler.Common.Models;
using Console = System.Console;

namespace WebCrawler.Cli.Lib;

public class WebCrawlerService : IWebCrawlerService
{
    //Services
    private readonly IWebpageService _webpageService;
    private readonly IHtmlParser _htmlParser;

    //State
    private readonly Queue<CrawlerJob> _queue;
    private readonly List<string> _allValidKnownUrls;
    private Uri _rootSite;
    
    private readonly List<string> _invalidQualifiers = new() { "mailto:", "sms:", "tel:", "#", "sms:", "javascript:", "monzo:" };


    public WebCrawlerService(IWebpageService webpageService, IHtmlParser htmlParser)
    {
        _queue = new Queue<CrawlerJob>();
        _webpageService = webpageService;
        _htmlParser = htmlParser;
        _allValidKnownUrls = new List<string>();
    }

    public async Task RunAsync(string urlTarget)
    {
        Console.WriteLine($"Starting Crawler at {urlTarget}.");

        //Setup initial data
        _queue.Enqueue(new CrawlerJob(urlTarget));
        _allValidKnownUrls.Add(urlTarget);
        _rootSite = new Uri(urlTarget);
        
        while (_queue.Count > 0)
        {
            var job = _queue.Dequeue();
            var urls = await ProcessPage(job);
            OutputResult(job.Url, urls);
        }
        Console.WriteLine("Crawler Complete.");
    }

    private void OutputResult(string url, List<string> results)
    {
        Console.WriteLine("---------------");
        Console.WriteLine(url);
        Console.WriteLine("---------------");
        if (!results.Any())
        {
            Console.WriteLine("No links found.");
        }
        else
        {
            results.ForEach(Console.WriteLine);
        }
        Console.WriteLine("---------------");
        Console.WriteLine();
    }

    private async Task<List<string>> ProcessPage(CrawlerJob job)
    {
        var pageContents = await _webpageService.GetWebpage(job.Uri);

        if (pageContents == null)
        {
            return new List<string>();
        }

        var parsedLinks = await _htmlParser.ParseLinks(pageContents);

        var results = new List<string>();

        // Add valid links to result and unvisited links to queue
        foreach (var link in parsedLinks)
        {
            if (IsProcessableLink(link))
            {
                // If the link can't be processed, skip processing it

                Uri? processedUri = null;

                //Check if link is fully qualified then construct the link, appending if it's a relative link
                if (link.ToLower().StartsWith("http:") || link.ToLower().StartsWith("https:"))
                {
                    processedUri = new Uri(link);
                }
                else
                {
                    //If the link starts with a / then it's relative to the root otherwise it's relative to the current location
                    if (link.StartsWith("/"))
                    {
                        processedUri = _rootSite.Append(link);
                    }
                    else
                    {
                        processedUri = job.Uri.Append(link);
                    }
                    //processedUri = link.StartsWith("/") ? _rootSite.Append(link) : job.Uri.Append(link);
                }

                if (job.Uri.Host != processedUri.Host)
                {
                    //If the host of the url to process isn't the same host as we are already crawling, skip it
                    continue;
                }

                var uriString = processedUri.ToString();
                //If we've not previously found the url (or it's not the url we are on right now) then we can add it to the queue and list of known urls
                if (!_allValidKnownUrls.Any(x => x.Equals(uriString)))
                {
                    _queue.Enqueue(new CrawlerJob(uriString));
                    _allValidKnownUrls.Add(uriString);
                }
            }

            //Add the link to the results if it hasn't already been added (there might be duplicates on the same page)
            if (!results.Any(x => x.Equals(link)))
            {
                results.Add(link);
            }
        }

        return results;
    }

    private bool IsProcessableLink(string link)
    {
        var isProcessable = true;

        var lowerCaseLink = link.ToLower();
        //If the links is null or empty, it's not valid
        if (string.IsNullOrEmpty(link))
        {
            isProcessable = false;
        }
        // If the url starts with an invalid qualifier then it is not something we can/will scrape
        else if (_invalidQualifiers.Any(x => lowerCaseLink.StartsWith(x)))
        {
            isProcessable = false;
        }

        return isProcessable;
    }
}
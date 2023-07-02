using System.Collections.Concurrent;
using System.Diagnostics;
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
    private readonly ManagedBlockingCollection<CrawlerJob> _queue;
    private readonly ConcurrentDictionary<string, Uri> _allValidKnownUrls;
    private Uri _rootSite;

    private readonly List<string> _invalidQualifiers =
        new() { "mailto:", "sms:", "tel:", "#", "sms:", "javascript:", "monzo:" };

    private readonly List<string> _fullQualifiers = new() { "http:", "https:" };

    public WebCrawlerService(IWebpageService webpageService, IHtmlParser htmlParser)
    {
        _queue = new ManagedBlockingCollection<CrawlerJob>();
        _webpageService = webpageService;
        _htmlParser = htmlParser;
        _allValidKnownUrls = new ConcurrentDictionary<string, Uri>();
    }


    public async Task RunAsync(string urlTarget)
    {
        Console.WriteLine($"Starting Crawler at {urlTarget}.");

        //Setup initial data
        _queue.Add(new CrawlerJob(urlTarget));
        _rootSite = new Uri(urlTarget);
        _allValidKnownUrls.TryAdd(urlTarget, _rootSite);
        
        //Timer to provider processing duration
        var timer = new Stopwatch();
        timer.Start();

        _queue.BeginObservingAutoComplete();

        var actions = Enumerable.Repeat(() =>
        {
            foreach (var job in _queue.GetConsumingEnumerable())
            {
                var urls = ProcessPage(new CrawlerJob(job.Url)).Result;
                OutputResult(job.Url, urls);
            }
        }, 16).ToArray();
        
        while (!_queue.IsCompleted)
        {
            Parallel.Invoke(actions);
        }
        
        timer.Stop();
        Console.WriteLine("Time taken: " + timer.Elapsed.ToString(@"m\:ss\.fff"));
        Console.WriteLine($"{_allValidKnownUrls.Count} unique urls within the domain found.");
        Console.WriteLine("Crawler Complete.");
    }

    private void OutputResult(string url, List<string> results)
    {
        Console.Write(
            "\n------------\n" +
            url +
            "\n------------\n" +
            (!results.Any() ? "No Links Found." : string.Join("\n", results )) +
            "\n------------\n"
        );
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
                if (_fullQualifiers.Any(x => link.ToLower().StartsWith(x)))
                {
                    processedUri = new Uri(link);
                }
                else
                {
                    //If the link starts with a www. then we should append a scheme
                    if (link.StartsWith("www."))
                    {
                        processedUri = new Uri($"https://{link}");
                    }
                    //if it begins wtih / then it's relative to the root
                    else if (link.StartsWith("/"))
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
                //If we've not previously found the url then we can add it to the queue and list of known urls
                if (_allValidKnownUrls.TryAdd(uriString, processedUri))
                {
                    //If we've been able to add it to the dictionary then it's a new url so we should also crawl it
                    //barrier.AddParticipant();
                    _queue.Add(new CrawlerJob(uriString));
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
using HtmlAgilityPack;

namespace WebCrawler.Cli.Lib;

public class HtmlParser : IHtmlParser
{
    public async Task<IEnumerable<string>> ParseLinks(string html)
    {
        //If there is no text to parse, return an empty list
        if (string.IsNullOrEmpty(html))
        {
            return new List<string>();
        }
        
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);
        
        //Select all 'a' Tags that have a href attribute
        var htmlNodes = htmlDoc.DocumentNode.SelectNodes("//a[@href]");

        
        //If there are no links, return an empty list
        if (htmlNodes == null)
        {
            return new List<string>();
        }

        return htmlNodes.Select(x => x.Attributes.Single(y=> y.Name == "href").Value);
    }
}
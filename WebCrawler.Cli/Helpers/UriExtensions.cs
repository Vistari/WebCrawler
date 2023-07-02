namespace WebCrawler.Cli.Helpers;

public static class UriExtensions
{
    public static Uri Append(this Uri uri, params string[] paths)
    {

        //Split up the current URI into sections
        var uriSections = uri.PathAndQuery.Split("/").ToList();

        //Add all of the sections of the paths we are appending
        foreach (var path in paths)
        {
            uriSections.AddRange(path.Split("/"));
        }
        
        //Remove any null/empty entries
        uriSections = uriSections.Where(x => !string.IsNullOrEmpty(x)).ToList();
        
        var uriBuilder = new UriBuilder()
        {
            Scheme = uri.Scheme,
            Host = uri.Host
        };

        return new Uri(uriBuilder.Uri, string.Join("/", uriSections));
    }
}
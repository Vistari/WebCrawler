using CommandLine;

namespace WebCrawler.Cli.Config;

public class CmdLineArguments
{
    [Option('t',"target", Required = true, HelpText = "The target URL to crawl")]
    public string Target { get; set; }
}
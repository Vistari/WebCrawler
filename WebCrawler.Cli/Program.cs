using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebCrawler.Cli.Config;
using WebCrawler.Cli.Lib;

namespace WebCrawler.Cli;

public class Program
{
    public static async Task Main(string[] args)
    {
        using var host = CreateHostBuilder(args).Build();

        await host.StartAsync();

        using var scope = host.Services.CreateScope();
        var worker = scope.ServiceProvider.GetService<IWebCrawlerService>();

        await Parser.Default.ParseArguments<CmdLineArguments>(args).WithParsedAsync(async parsedArgs =>
        {
            await worker!.RunAsync(parsedArgs.Target);
        });

        await host.StopAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddScoped<IWebCrawlerService, WebCrawlerService>();
                services.Configure<CrawlerOptions>(hostContext.Configuration.GetSection(CrawlerOptions.SectionName));
            });
    }
}
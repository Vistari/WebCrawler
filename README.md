# Webcrawler
A .NET-based Web Crawler.

# Requirements to run/build locally
- [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks) - The solution is written entirely in C# .Net. The SDK should provide everything you need to build and run the solution. Any other dependencies this has will be installed by the restore step of the build process.
- [Docker](https://www.docker.com/products/docker-desktop/) (optional) - There is a docker container available for this if you would prefer to run it from there.

## Build Docker image
Run the following command from the solution directory to build the docker image:
```sh
docker build --tag 'webcrawler:local' -f .\WebCrawler.Cli\Dockerfile .
```

## Run the Docker image
Run the following command to run the container (in the example this will target Google)
```sh
docker run 'webcrawler:local' --target https://www.google.co.uk/
```

You can get help with the available options running:
```sh
docker run 'webcrawler:local' --help
```
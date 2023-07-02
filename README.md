# Webcrawler


## Build Docker image
Run the following command from the solution directory to build the docker image:
```dockerfile
docker build --tag 'webcrawler:local' -f .\WebCrawler.Cli\Dockerfile .
```

## Run the Docker image
Run the following command to run the container (in the example this will target Google)
```dockerfile
docker run 'webcrawler:local' --target https://www.google.co.uk/
```
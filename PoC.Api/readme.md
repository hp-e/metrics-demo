# demo api

```
docker build --pull --rm -f "Dockerfile" -t poc:latest "."
```

```
docker run --rm -d -p 5100:5100 poc:latest -n pocapi
```

go to http://localhost:5100/health to see the health of the api
and http://localhost:5100/metrics-text to see all exposed metrics
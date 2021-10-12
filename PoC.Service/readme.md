# demo api

```
docker build --pull --rm -f "Dockerfile" -t poc-service:latest "."
```

```
docker run --rm -d -p 5200:5200 poc-service:latest -n pocservice
```

go to http://localhost:5200/health to see the health of the service
and http://localhost:5200/metrics-text to see all exposed metrics
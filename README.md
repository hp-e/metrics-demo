# Metrics demo

Use Prometheus, Loki to collect metrics about applications.
Use Grafana to show dashboards with the collected metrics.

Use docker compose to build the services and download prometheus, grafana and loki.
```cmd
docker-compose up -d
```

Note that this works best on Linux

## Grafana

Is used to show dashboards with metrics. There are two dasboards that is functioning.

- Service Health: Shows the health of the services
- App Metrics - Web Monitoring - Prometheus: Standard metrics collected from the services

Use the following url to access grafana: ```http://localhost:3000```
```
user: admin
password: pwd
```
In the ```./dashboards``` there is a logging dashboard. This is not implemented.

## Demo applications

**PoC.Api** 

An API with the only purpose to generate log entries and metrics.

Runs on ```http://localhost:5400``` and exposes the following endpoints
- Health: ```http://localhost:5400/health```
- metrics: ```http://localhost:5400/metrics``` or ```http://localhost:5400/metrics-text```
- Swagger: ```http://localhost:5400/swagger/v1```
- Log info: ```http://localhost:5400/api/logger/info```
- Log error: ```http://localhost:5400/api/logger/error```

**PoC.Service** 

A service with the only purpose to generate log entries and metrics.

Runs on ```http://localhost:5500``` and exposes the following endpoints
- Health: ```http://localhost:5500/health```. The health is randomized between being healthy or unhealthy
- metrics: ```http://localhost:5500/metrics``` or ```http://localhost:5500/metrics-text```


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PoC.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMetrics _metrics;

        public Worker(ILogger<Worker> logger, IMetrics metrics)
        {
            _logger = logger;
            _metrics = metrics;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Random rnd = new Random();
            while (!stoppingToken.IsCancellationRequested)
            {
                using (_metrics.Measure.Timer.Time(MetricsRegistry.RequestTime("ExecuteAsync", "Worker")))
                {

                    int millis = rnd.Next(1000, 10000);

                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                    _metrics.Measure.Counter.Increment(MetricsRegistry.CustomCounter("WorkerCount", "ExecuteAsync", "Worker"));
                    await Task.Delay(millis, stoppingToken);
                }

            }
        }
    }
}

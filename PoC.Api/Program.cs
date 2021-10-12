using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.Grafana.Loki;

namespace PoC.Api
{
    public class Program
    {

        private static Logger _logger;
        private static IConfigurationRoot _config;

        public static void Main(string[] args)
        {
            var env = GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env}.json", true)
                .Build();

            _logger = new LoggerConfiguration()
                .Filter.ByExcluding(c => c.Properties.Any(p => p.Value.ToString().Contains("metrics-text")))
                .Enrich.FromLogContext()
                .Enrich.WithAssemblyName()
                .Enrich.WithAssemblyVersion()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithExceptionData()
                .WriteTo.GrafanaLoki(_config.GetValue<string>("LokiUrl"))
                .WriteTo.Console()
                .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog(_logger)
                .UseMetricsWebTracking()
                .UseMetrics(options =>
                {
                    options.EndpointOptions = endPointOptions =>
                    {
                        endPointOptions.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
                        endPointOptions.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
                        endPointOptions.EnvironmentInfoEndpointEnabled = false;
                    };
                })
                .ConfigureServices(ConfigureServices)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseKestrel()
                        .UseUrls($"http://0.0.0.0:5100")
                        .UseStartup<Startup>();
                });

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddAppMetricsHealthPublishing();
        }
        
        private static string GetEnvironmentVariable(string name, string defaultValue = null) =>
            Environment.GetEnvironmentVariable(name) ?? defaultValue;
    }
}

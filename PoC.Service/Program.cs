using System;
using System.Linq;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.Grafana.Loki;

namespace PoC.Service
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
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHealthChecks();
                    services.AddHostedService<Worker>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseKestrel()
                        .UseUrls($"http://0.0.0.0:5000")                        
                        .UseStartup<Startup>();
                });

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddAppMetricsHealthPublishing();
        }

        private static string GetEnvironmentVariable(string name, string defaultValue = null) =>
            Environment.GetEnvironmentVariable(name) ?? defaultValue;
    }


    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<ServiceHealthCheck>("service_health_check");
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           
            app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health").AllowAnonymous();
            });
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Timer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PoC.Service
{
    public class MetricsRegistry
    {
        private static readonly string _version = "";
        private static readonly string _context = "poc";
        private static readonly string _methodName = "methodName";
        
        public static MetricTags CreateTagsArrays(string methodName, string className = null)
        {
            var keys = new List<string>();
            var values = new List<string>();

            Assembly execAssembly = Assembly.GetEntryAssembly();

            AssemblyName name = execAssembly.GetName();

            if (!string.IsNullOrEmpty(methodName))
            {
                keys.Add(_methodName);
                values.Add(methodName);
            }
            
            if (!string.IsNullOrEmpty(className))
            {
                keys.Add("className");
                values.Add(className);
            }
            // if (!string.IsNullOrEmpty(version))
            // {
                keys.Add("version");
                // var v = $"{name.Version.Major}.{name.Version.Minor}.{name.Version.Build}";
                values.Add(name.Version.ToString());
            // }

            if (keys.Count() > 0 && keys.Count() == values.Count())
            {
                return new MetricTags(keys.ToArray(), values.ToArray());
            }
            else
            {
                return new MetricTags();
            }
        }
       
        public static TimerOptions RequestTime(string methodName, string className = null) => new TimerOptions
        {
            Name = "Execution Timer",
            MeasurementUnit = Unit.Requests,
            RateUnit = TimeUnit.Milliseconds,
            DurationUnit = TimeUnit.Milliseconds,
            Context = _context,
            Tags = CreateTagsArrays(methodName, className)
        };

        public static CounterOptions CustomCounter(string name, string methodName, string className = null) => new CounterOptions
        {
            Name = name,
            Context = _context,
            MeasurementUnit = Unit.Calls,
            Tags = CreateTagsArrays(methodName, className)
        };

        

    }
}

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace WebApiExample.Extensions
{
    public static partial class Extensions
    {
        const string serviceName = "WebApiExample";
        public static void AddOpenTelemetry(this IServiceCollection services, ILoggingBuilder logging, ConfigurationManager configuration)
        {
            logging.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(serviceName))
                    .AddConsoleExporter();
            });

            services.AddOpenTelemetry()
                  .ConfigureResource(resource => resource.AddService(serviceName))
                  .WithTracing(tracing => tracing
                      .AddAspNetCoreInstrumentation()
                      .AddHttpClientInstrumentation()
                      .AddEntityFrameworkCoreInstrumentation()
                      .AddConsoleExporter()
                      .AddZipkinExporter(opt => opt.Endpoint = new Uri("http://localhost:9411/api/v2/spans")))
                  .WithMetrics(metrics => metrics
                      .AddAspNetCoreInstrumentation()
                      .AddConsoleExporter());
        }
    }
}

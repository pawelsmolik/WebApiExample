using Serilog;

namespace WebApiExample.Extensions
{
    public static partial class Extensions
    {
        public static void AddSerilog(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddSerilog((services, lc) => lc
            .ReadFrom.Configuration(configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());
        }
    }
}

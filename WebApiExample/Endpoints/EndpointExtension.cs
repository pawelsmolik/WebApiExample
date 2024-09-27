namespace WebApiExample.Endpoints
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Options;
    using Serilog;

    /// <summary>
    /// Defines the <see cref="EndpointExtension" />
    /// </summary>
    public static class EndpointExtension
    {
        /// <summary>
        /// The AddEndpoints
        /// </summary>
        /// <param name="app">The app<see cref="WebApplication"/></param>
        public static void AddEndpoints(this WebApplication app)
        {
            app.MapGet("/error", [Authorize(Policy = "Admin")] (IOptions<AppSettings> appSettings, Serilog.ILogger logger, HttpContext ctx) =>
            {
                logger.Error("Log error");
                Log.Warning("Log warning");
                return new
                {
                    data = "Error Result"
                };
            })
            .WithName("ErrorTest")
            .WithDescription("Test display error in logs")
            .WithOpenApi()
            .RequireAuthorization();

            app.MapGet("/GetCarList", [Authorize(Policy = "Admin")] (IOptions<AppSettings> appSettings, Serilog.ILogger logger, HttpContext ctx) =>
            {
                var response = new List<string> {
                    "Opel", "Mercedes", "Volvo"
                };

                return new
                {
                    response
                };
            })
            .WithName("GetCarList")
            .WithDescription("Return Car List")
            .WithOpenApi()
            .RequireAuthorization();

            app.MapGet("/version", (IOptions<AppSettings> appSettings, Serilog.ILogger logger, HttpContext ctx) =>
            {
                var result = $"Wersja {appSettings.Value.Version}";
                return new
                {
                    result
                };
            })
            .WithName("GetVersion")
            .WithDescription("Return App Version")
            .WithOpenApi();
        }
    }
}

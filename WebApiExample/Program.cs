using Microsoft.Extensions.Options;
using Serilog;
using WebApiExample;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("starting server.");
    var configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json")
           .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
           .Build();

    var builder = WebApplication.CreateBuilder(args);
    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddSerilog((services, lc) => lc
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

    var app = builder.Build();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();

    app.MapGet("/error", (IOptions<AppSettings> appSettings, Serilog.ILogger logger) =>
    {
        logger.Error("Log error");
        Log.Warning("Log warning");
        throw new Exception("test error");
        return appSettings.Value.Version;
    })
    .WithName("ErrorTest")
    .WithDescription("Test display error in logs")
    .WithOpenApi();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "server terminated unexpectedly");
}
finally 
{
    Log.CloseAndFlush();
}


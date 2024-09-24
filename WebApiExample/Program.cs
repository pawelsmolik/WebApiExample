using Serilog;
using WebApiExample;
using WebApiExample.Extensions;
using WebApiExample.Endpoints;

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
    builder.Services.AddSwagger();
    builder.Services.AddSerilog(builder.Configuration);
    builder.Services.AddOpenTelemetry(builder.Logging, builder.Configuration);
    builder.Services.AddAuthentication(builder.Configuration);
    builder.Services.AddAuthorization(builder.Configuration);
    builder.Services.AddCors(builder.Configuration);

    var app = builder.Build();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
    app.UseCors("Angular");
    app.UseAuthentication();
    app.UseAuthorization();
    app.AddEndpoints();
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


using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using WebApiExample;
using Microsoft.AspNetCore.Authorization;
using IdentityModel.AspNetCore.OAuth2Introspection;

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
    builder.Services.AddSwaggerGen(opt =>
    {
        opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
        opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });

        opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
    });

    builder.Services.AddSerilog((services, lc) => lc
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

    //Authhentication without Introspection
    /*
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 // base-address of your identityserver
                 options.Authority = "https://localhost:7130";

                 // audience is optional, make sure you read the following paragraphs
                 // to understand your options
                 //options.TokenValidationParameters.ValidateAudience = false;

                 // it's recommended to check the type header to avoid "JWT confusion" attacks
                 //options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
             });
    */

    builder.Services.AddAuthentication(OAuth2IntrospectionDefaults.AuthenticationScheme)
    .AddOAuth2Introspection(options =>
    {
        options.Authority = "https://localhost:7130";
        options.ClientId = "WebApiExample";
        options.ClientSecret = "ABC123";
    });

    ;
    builder.Services.AddAuthorization(options => {
        options.AddPolicy("Admin", policy => policy.RequireClaim("Role", "Admin"));
    });

    builder.Services.AddCors(o => o.AddPolicy("Angular", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    }));

    var app = builder.Build();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
    app.UseCors("Angular");
    app.UseAuthentication();
    app.UseAuthorization();

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


using IdentityModel.AspNetCore.OAuth2Introspection;

namespace WebApiExample.Extensions
{
    public static partial class Extensions
    {
        public static void AddAuthentication(this IServiceCollection services, ConfigurationManager configuration)
        {
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

            var config = configuration.GetSection("AppSettings:Authentication:Introspection");

            services.AddAuthentication(OAuth2IntrospectionDefaults.AuthenticationScheme)
            .AddOAuth2Introspection(options =>
            {
                options.Authority = config.GetValue("Authority", "");
                options.ClientId = config.GetValue("ClientId", "");
                options.ClientSecret = config.GetValue("ClientSecret", "");
            });
        }
    }
}

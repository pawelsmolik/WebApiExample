namespace WebApiExample.Extensions
{
    public static partial class Extensions
    {
        public static void AddAuthorization(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddAuthorization(options => {
                options.AddPolicy("Admin", policy => policy.RequireClaim("Role", "Admin"));
            });
        }
    }
}

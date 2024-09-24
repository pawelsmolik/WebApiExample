namespace WebApiExample.Extensions
{
    public static partial class Extensions
    {
        public static void AddCors(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddCors(o => o.AddPolicy("Angular", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
        }
    }
}

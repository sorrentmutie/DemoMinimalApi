using MyApi.Configurations;

namespace MyApi.ExtensionMethods;

public static class ApplicationServices
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("miaconnectionstring");
        services.AddSwaggerGen();
        services.AddScoped<ITodoItems, MockItems>();
        services.AddScoped<IProducts, ProductsService>();
        services.AddDbContext<NorthwindContext>(
             x => x.UseSqlServer(connectionString));
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));


    }
}
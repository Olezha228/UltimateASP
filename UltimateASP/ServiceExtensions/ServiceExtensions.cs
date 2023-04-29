using Microsoft.EntityFrameworkCore;
using Repository;

namespace UltimateASP.ServiceExtensions;

public static class ServiceExtensions
{
    public static void ConfigureServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.ConfigureDI();
        services.ConfigureCors();
        services.ConfigureIISIntegration();
        services.AddControllers();
        services.ConfigureSqlContext(configuration);
        services.ConfigureControllers();
        services.AddAutoMapper(typeof(Program));

    }

    private static void ConfigureCors(this IServiceCollection services) =>
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

    // ReSharper disable once InconsistentNaming
    private static void ConfigureIISIntegration(this IServiceCollection services) =>
        services.Configure<IISOptions>(options =>
        {
        });

    private static void ConfigureSqlContext(this IServiceCollection services,
        IConfiguration configuration) =>
        services.AddDbContext<RepositoryContext>(opts =>
            opts.UseSqlServer(configuration.GetConnectionString("sqlConnection")));

    // ReSharper disable once InconsistentNaming
    private static void ConfigureControllers(this IServiceCollection services) =>
        services.AddControllers()
            .AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);




}
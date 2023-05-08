using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Repository;
using UltimateASP.Formatters;

namespace UltimateASP.ServiceExtensions;

public static class ServiceExtensions
{
    public static void ConfigureServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.ConfigureDI();

        services.ConfigureCors();
        services.ConfigureIISIntegration();
        services.ConfigureSqlContext(configuration);
        services.ConfigureApiBehaviorOptions();
        services.ConfigureControllers();
        services.AddAutoMapper(typeof(Program));
    }

    private static void ConfigureCors(this IServiceCollection services) =>
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination"));
        });

    // ReSharper disable once InconsistentNaming
    private static void ConfigureIISIntegration(this IServiceCollection services) =>
        services.Configure<IISOptions>(options => { });

    private static void ConfigureSqlContext(this IServiceCollection services,
        IConfiguration configuration) =>
        services.AddDbContext<RepositoryContext>(opts =>
            opts.UseSqlServer(configuration.GetConnectionString("sqlConnection")));

    private static void ConfigureControllers(this IServiceCollection services) =>
        services.AddControllers(config =>
            {
                config.RespectBrowserAcceptHeader = true;
                config.ReturnHttpNotAcceptable = true;
                config.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
            }).AddXmlDataContractSerializerFormatters()
            .AddCustomCsvFormatter()
            .AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);

    public static IMvcBuilder AddCustomCsvFormatter(this IMvcBuilder builder) =>
        builder.AddMvcOptions(config => config.OutputFormatters.Add(new
            CsvOutputFormatter()));

    public static void ConfigureApiBehaviorOptions(this IServiceCollection services) =>
        services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
    
    static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
        new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
            .Services.BuildServiceProvider()
            .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
            .OfType<NewtonsoftJsonPatchInputFormatter>().First();
}
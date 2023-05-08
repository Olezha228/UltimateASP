using CompanyEmployees.Presentation.ActionFilters;
using Contracts.Manager;
using Repository.Manager;
using Service;
using Service.Contracts.Manager;
using Service.Manager;

namespace UltimateASP.ServiceExtensions;

public static class DIConfiguration
{
    public static void ConfigureDI(this IServiceCollection services)
    {
        services.ConfigureLoggerService();
        services.ConfigureRepositoryManager();
        services.ConfigureServiceManager();
        services.AddTransient<ServiceHelper>();
        services.ConfigureValidationFilter();
        services.ConfigureJsonPatchValidationFilter();
    }

    private static void ConfigureLoggerService(this IServiceCollection services) =>
        services.AddSingleton<ILoggerManager, LoggerManager>();

    private static void ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();

    private static void ConfigureServiceManager(this IServiceCollection services) =>
        services.AddScoped<IServiceManager, ServiceManager>();

    private static void ConfigureValidationFilter(this IServiceCollection services) =>
        services.AddScoped<ValidationFilterAttribute>();

    private static void ConfigureJsonPatchValidationFilter(this IServiceCollection services) =>
        services.AddScoped<JsonPatchValidationFilter>();

}
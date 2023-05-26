using CompanyEmployees.Presentation.ActionFilters;
using Contracts.Contracts;
using Contracts.Manager;
using Repository.Manager;
using Service;
using Service.Contracts.Manager;
using Service.DataShaping;
using Service.Manager;
using Shared.DataTransferObjects.Employee;
using UltimateASP.Utility;

namespace UltimateASP.ServiceExtensions;

public static class DIConfiguration
{
    public static void ConfigureDI(this IServiceCollection services)
    {
        services.ConfigureLoggerService();
        services.ConfigureRepositoryManager();
        services.ConfigureServiceManager();
        services.ConfigureServiceHelper();
        services.ConfigureValidationFilter();
        services.ConfigureJsonPatchValidationFilter();
        services.ConfigureValidateMediaTypeAttribute();
        services.ConfigureEmployeeDataShaping();
        services.ConfigureEmployeeLinks();
    }

    private static void ConfigureLoggerService(this IServiceCollection services) =>
        services.AddSingleton<ILoggerManager, LoggerManager>();

    private static void ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();

    private static void ConfigureServiceManager(this IServiceCollection services) =>
        services.AddScoped<IServiceManager, ServiceManager>();

    private static void ConfigureServiceHelper(this IServiceCollection services) =>
        services.AddTransient<ServiceHelper>();

    private static void ConfigureValidationFilter(this IServiceCollection services) =>
        services.AddScoped<ValidationFilterAttribute>();

    private static void ConfigureJsonPatchValidationFilter(this IServiceCollection services) =>
        services.AddScoped<JsonPatchValidationFilter>();

    private static void ConfigureValidateMediaTypeAttribute(this IServiceCollection services) =>
        services.AddScoped<ValidateMediaTypeAttribute>();

    private static void ConfigureEmployeeDataShaping(this IServiceCollection services) =>
        services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();

    private static void ConfigureEmployeeLinks(this IServiceCollection services) =>
        services.AddScoped<IEmployeeLinks, EmployeeLinks>();

}
using Contracts;
using Repository.Manager;
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
    }

    private static void ConfigureLoggerService(this IServiceCollection services) =>
        services.AddSingleton<ILoggerManager, LoggerManager>();

    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();

    public static void ConfigureServiceManager(this IServiceCollection services) =>
        services.AddScoped<IServiceManager, ServiceManager>();

}
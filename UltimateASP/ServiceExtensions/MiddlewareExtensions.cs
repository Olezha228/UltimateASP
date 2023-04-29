using UltimateASP.ServiceExtensions.Middleware;

namespace UltimateASP.ServiceExtensions; 

public static class MiddlewareExtension
{
    public static void UseAllMiddlewares(this WebApplication app)
    {
        app.ConfigureExceptionHandler(GetLogger(app));

        if (app.Environment.IsProduction())
            app.UseHsts();

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }

    private static ILoggerManager GetLogger(WebApplication app)
    {
        return app.Services.GetRequiredService<ILoggerManager>();
    }
}

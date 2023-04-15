namespace UltimateASP.ServiceExtensions; 

public static class MiddlewareExtension
{
    public static void UseAllMiddlewares(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }
}

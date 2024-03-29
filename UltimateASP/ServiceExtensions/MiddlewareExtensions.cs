﻿using AspNetCoreRateLimit;
using Microsoft.AspNetCore.HttpOverrides;
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
        app.UseStaticFiles();

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        });

        // desirably before UseCors
        app.UseIpRateLimiting();

        app.UseCors("CorsPolicy");
        app.UseResponseCaching(); // this one is gotta go after UseCors! always!
        app.UseHttpCacheHeaders();

        app.UseAuthentication(); // this one is gotta go before UseAuthorization! always!
        app.UseAuthorization();
        app.MapControllers();
    }

    private static ILoggerManager GetLogger(IHost app)
    {
        return app.Services.GetRequiredService<ILoggerManager>();
    }
}

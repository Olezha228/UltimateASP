namespace UltimateASP.ServiceExtensions;

public static class LogConfiguration
{
    public static void Configure()
    {
        LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(),
            "/Config/nlog.config"));
    }
}
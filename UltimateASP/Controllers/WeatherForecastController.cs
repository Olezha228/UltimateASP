using Contracts.Manager;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Manager;

namespace UltimateASP.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private ILoggerManager _logger;
    private IRepositoryManager _repository;

    public WeatherForecastController(ILoggerManager logger, IRepositoryManager repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpGet]
    public void Get()
    {
        _logger.LogInfo("Here is info message from our values controller.");
        _logger.LogDebug("Here is debug message from our values controller.");
        _logger.LogWarn("Here is warn message from our values controller.");
        _logger.LogError("Here is an error message from our values controller.");
    }

}
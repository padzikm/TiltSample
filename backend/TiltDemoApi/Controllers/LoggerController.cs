using Microsoft.AspNetCore.Mvc;

namespace TiltDemoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class LoggerController : ControllerBase
{
    private readonly ILogger<LoggerController> _logger;

    public LoggerController(ILogger<LoggerController> logger)
    {
        _logger = logger;
    }

    [HttpGet("info/{msg}")]
    public ActionResult LogInfo(string msg)
    {
        _logger.LogInformation(msg);
        return Ok();
    }
    
    [HttpGet("warning/{msg}")]
    public ActionResult LogWarning(string msg)
    {
        _logger.LogWarning(msg);
        return Ok();
    }
    
    [HttpGet("error/{msg}")]
    public ActionResult LogError(string msg)
    {
        _logger.LogError(msg);
        return Ok();
    }
}
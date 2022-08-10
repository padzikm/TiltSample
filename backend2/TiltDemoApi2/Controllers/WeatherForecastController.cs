using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiltDemoApi2.Database;

namespace TiltDemoApi2.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly AppDbContext _dbContext;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet("bla")]
    public string Get2()
    {
        return "Hello world20";
    }

    [HttpGet("savedb/{age}")]
    public async Task<ActionResult> SaveData(int age)
    {
        var m = new Model() { Date = DateTime.Now, Age = age };
        _dbContext.SecondData.Add(m);
        await _dbContext.SaveChangesAsync();
        
        return Ok(m);
    }
    
    [HttpGet("getdb/{age}")]
    public async Task<ActionResult> GetData(int age)
    {
        var m = await _dbContext.SecondData.Where(p => p.Age == age).ToListAsync();
        
        return Ok(m);
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiltDemoApi.Database;

namespace TiltDemoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AppDbContext _dbContext;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpClientFactory httpClientFactory, AppDbContext dbContext)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
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
    public async Task<string> Get2()
    {
        var httpClient = _httpClientFactory.CreateClient();
        var res = await httpClient.GetAsync("http://back2-svc.app2/WeatherForecast/bla");
        var t = await res.Content.ReadAsStringAsync();
        return t;
        // return "Hello world16";
    }
    
    [HttpGet("savedb/{name}")]
    public async Task<ActionResult> SaveData(string name)
    {
        var m = new Model() { Date = DateTime.Now, Name = name };
        _dbContext.FirstData.Add(m);
        await _dbContext.SaveChangesAsync();
        
        return Ok(m);
    }
    
    [HttpGet("getdb/{name}")]
    public async Task<ActionResult> GetData(string name)
    {
        var m = await _dbContext.FirstData.Where(p => p.Name == name).ToListAsync();
        
        return Ok(m);
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TiltDemoApi.Configuration;
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
    private readonly IConfiguration _config;
    private readonly IOptions<ConfigBack2> _configback2;
    private readonly IOptions<ConfigFront> _configfront;
    private readonly IOptions<ConfigMap> _configmap;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpClientFactory httpClientFactory, AppDbContext dbContext, IConfiguration config,
        IOptions<ConfigBack2> configback2, IOptions<ConfigFront> configfront, IOptions<ConfigMap> configmap)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _dbContext = dbContext;
        _config = config;
        _configback2 = configback2;
        _configfront = configfront;
        _configmap = configmap;
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
        var baseurl = _config["Back2_Base_Url"];
        var url = $"{baseurl}/WeatherForecast/bla";
        var httpClient = _httpClientFactory.CreateClient();
        var res = await httpClient.GetAsync(url);
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
    
    [HttpGet("config/inline")]
    public ActionResult GetConfigInline()
    {
        var cfg = _config.GetSection(ConfigHost.Key).Get<ConfigHost>();
        
        return cfg == null ? Ok("null") : Ok(cfg);
    }
    
    [HttpGet("config/opt")]
    public ActionResult GetConfigOpt()
    {
        var cfg = _configback2.Value;
        
        return cfg == null ? Ok("null") : Ok(cfg);
    }
    
    [HttpGet("config/valid")]
    public ActionResult GetConfigValid()
    {
        var cfg = _configfront.Value;
        
        return cfg == null ? Ok("null") : Ok(cfg);
    }
    
    [HttpGet("config/map")]
    public ActionResult GetConfigMap()
    {
        var cfg = _configmap.Value;
        Console.WriteLine("cokolwiek");
        
        return cfg == null ? Ok("null") : Ok(cfg);
    }
}

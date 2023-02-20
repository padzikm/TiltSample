using System.Diagnostics;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using TiltDemoApi.Configuration;
using TiltDemoApi.Database;
using TiltDemoApi.MsgContracts;

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
    private readonly IPublishEndpoint _bus;
    private readonly IMessageScheduler _msgScheduler;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpClientFactory httpClientFactory, AppDbContext dbContext, IConfiguration config,
        IOptions<ConfigBack2> configback2, IOptions<ConfigFront> configfront, IOptions<ConfigMap> configmap, IPublishEndpoint bus, IMessageScheduler msgScheduler)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _dbContext = dbContext;
        _config = config;
        _configback2 = configback2;
        _configfront = configfront;
        _configmap = configmap;
        _bus = bus;
        _msgScheduler = msgScheduler;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        ActivitySource activitySource = new ActivitySource("back1.getweather");
        using (var ac = activitySource.StartActivity("important business"))
        {
            ac.SetTag("cos", "val1");
            _logger.LogInformation("testowe info");
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();
        }
    }

    [HttpGet("exl")]
    public async Task<string> GetExL()
    {
        throw new ApplicationException("sth went wrong");
    }
    
    [HttpGet("exr")]
    public async Task<string> GetExR()
    {
        var baseurl = _config["Back2_Base_Url"];
        var url = $"{baseurl}/WeatherForecast/ex";
        var httpClient = _httpClientFactory.CreateClient();
        var res = await httpClient.GetAsync(url);
        var t = await res.Content.ReadAsStringAsync();
        return t;
    }
    
    [HttpGet("bla")]
    public async Task<string> Get2()
    {
        ActivitySource activitySource = new ActivitySource("back1.bla");
        using (var ac = activitySource.StartActivity("remote call"))
        {
            Baggage.SetBaggage("temp", "asdfadsf");
            ac.SetTag("cos", "val2");
            var baseurl = _config["Back2_Base_Url"];
            var url = $"{baseurl}/WeatherForecast/bla";
            var httpClient = _httpClientFactory.CreateClient();
            var res = await httpClient.GetAsync(url);
            var t = await res.Content.ReadAsStringAsync();
            return t;
        }
    }
    
    [HttpGet("bla2")]
    public async Task<string> Get3()
    {
        ActivitySource activitySource = new ActivitySource("back1.bla2");
        using (var ac = activitySource.StartActivity("remote call"))
        {
            Baggage.SetBaggage("temp", "asdfadsf");
            ac.SetTag("cos", "val2");
            var baseurl = _config["Back2_Base_Url"];
            var url = $"{baseurl}/WeatherForecast/bla";
            var httpClient = _httpClientFactory.CreateClient();
            var res = await httpClient.GetAsync(url);
            var t = await res.Content.ReadAsStringAsync();
            var res2 = await httpClient.GetAsync(url);
            var t2 = await res2.Content.ReadAsStringAsync();
            return t + t2;
        }
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
    
    [HttpGet("savedb2/{age}")]
    public async Task<string> SaveData2(int age)
    {
        var baseurl = _config["Back2_Base_Url"];
        var url = $"{baseurl}/WeatherForecast/savedb/{age}";
        var httpClient = _httpClientFactory.CreateClient();
        var res = await httpClient.GetAsync(url);
        var t = await res.Content.ReadAsStringAsync();
        return t;
    }
    
    [HttpGet("getdb2/{age}")]
    public async Task<string> GetData2(int age)
    {
        var baseurl = _config["Back2_Base_Url"];
        var url = $"{baseurl}/WeatherForecast/getdb/{age}";
        var httpClient = _httpClientFactory.CreateClient();
        var res = await httpClient.GetAsync(url);
        var t = await res.Content.ReadAsStringAsync();
        return t;
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
        
        return cfg == null ? Ok("null") : Ok(cfg);
    }
    
    [HttpGet("user/{age}")]
    public async Task<ActionResult> UserEndpoint(int age)
    {
        var u = new CreateUser() { Age = age };
        await _bus.Publish<CreateUser>(u);
        return Ok();
    }
    
    [HttpGet("userdelay/{delay}/{age}")]
    public async Task<ActionResult> UserEndpointDelayed(int delay, int age)
    {
        var u = new CreateUser() { Age = age };
        await _msgScheduler.SchedulePublish<CreateUser>(TimeSpan.FromSeconds(delay), u);
        return Ok();
    }
}

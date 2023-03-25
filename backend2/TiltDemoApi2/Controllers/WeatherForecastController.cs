using System.Diagnostics;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using OpenTelemetry;
using TiltDemoApi.MsgContracts;
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
    private readonly MongoClient _mongoClient;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, AppDbContext dbContext, MongoClient mongoClient)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mongoClient = mongoClient;
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
        ActivitySource activitySource = new ActivitySource("back2.bla");
        using (var ac = activitySource.StartActivity("business"))
        {
            _logger.LogWarning("baggage");
            foreach (var item in Baggage.GetBaggage())
            {
                ac?.SetTag(item.Key, item.Value);
            }

            return "Hello world22";
        }
    }

    [HttpGet("savedb/{age}")]
    public async Task<ActionResult> SaveData(int age)
    {
        var m = new Model() { Date = DateTime.Now, Age = age, Id = age};
        var db = _mongoClient.GetDatabase("back2");
        var coll = db.GetCollection<Model>("sample");
        await coll.InsertOneAsync(m);
        
        // _dbContext.SecondData.Add(m);
        // await _dbContext.SaveChangesAsync();
        
        return Ok(m);
    }
    
    [HttpGet("getdb/{age}")]
    public async Task<ActionResult> GetData(int age)
    {
        // var m = await _dbContext.SecondData.Where(p => p.Age == age).ToListAsync();
        var db = _mongoClient.GetDatabase("back2");
        var coll = db.GetCollection<Model>("sample");
        var mr = await coll.FindAsync(p => p.Age == age);
        var m = mr.SingleOrDefault();
        
        return Ok(m);
    }

    [HttpGet("ex")]
    public async Task<string> GetExR()
    {
        throw new ApplicationException("sth went very wrong");
    }
}

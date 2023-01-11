using System.Diagnostics;
using MassTransit;
using TiltDemoApi.Database;
using TiltDemoApi.MsgContracts;

namespace TiltDemoApi.Msgs;

public class UserHandler : IConsumer<CreatedUser>
{
    private readonly ILogger<UserHandler> _logger;
    private readonly AppDbContext _dbContext;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;

    public UserHandler(ILogger<UserHandler> logger, AppDbContext dbContext, IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _logger = logger;
        _dbContext = dbContext;
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    public async Task Consume(ConsumeContext<CreatedUser> context)
    {
        ActivitySource activitySource = new ActivitySource("back1.msgs");
        using (var ac = activitySource.StartActivity("CreatedUser handler"))
        {
            _logger.LogInformation("created user received");
            _logger.LogInformation(context.Message.Name);
            var baseurl = _config["Back2_Base_Url"];
            var url = $"{baseurl}/WeatherForecast/getdb/{context.Message.Age}";
            var httpClient = _httpClientFactory.CreateClient();
            var res = await httpClient.GetAsync(url);
            var t = await res.Content.ReadAsStringAsync();
            _logger.LogInformation($"response from back2: {t}");
            var d = new Model() { Name = context.Message.Name, Date = DateTime.Now };
            _dbContext.FirstData.Add(d);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("created user saved");
        }
    }
}
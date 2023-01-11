using System.Diagnostics;
using MassTransit;
using TiltDemoApi.Database;
using TiltDemoApi.MsgContracts;

namespace TiltDemoApi.Msgs;

public class UserHandler : IConsumer<CreatedUser>
{
    private readonly ILogger<UserHandler> _logger;
    private readonly AppDbContext _dbContext;

    public UserHandler(ILogger<UserHandler> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CreatedUser> context)
    {
        ActivitySource activitySource = new ActivitySource("back1.msgs");
        using (var ac = activitySource.StartActivity("CreatedUser handler"))
        {
            _logger.LogInformation("created user received");
            _logger.LogInformation(context.Message.Name);

            var d = new Model() { Name = context.Message.Name, Date = DateTime.Now };
            _dbContext.FirstData.Add(d);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("created user saved");
        }
    }
}
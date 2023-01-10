using MassTransit;
using TiltDemoApi2.Database;
using TiltDemoApi.MsgContracts;

namespace TiltDemoApi2.Msgs;

public class UserHandler2 : IConsumer<CreateUser>
{
    private readonly ILogger<UserHandler2> _logger;
    private readonly AppDbContext _dbContext;

    public UserHandler2(ILogger<UserHandler2> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CreateUser> context)
    {
        _logger.LogInformation("create user received");
        _logger.LogInformation(context.Message.Age.ToString());

        var d = new Model() { Age = context.Message.Age, Date = DateTime.Now };
        _dbContext.SecondData.Add(d);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("create user saved");
        var n = $"name{context.Message.Age}";
        var u = new CreatedUser() { Name = n };
        await context.Publish<CreatedUser>(u);
        _logger.LogInformation("created user published");
    }
}
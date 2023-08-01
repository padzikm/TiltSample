using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Entities;

namespace ConsoleApp1.Migrations;

public class _006_FailCommand : IMigration
{
    public Task UpgradeAsync()
    {
        var cmd = new BsonDocument() { {"createe", "CmdCollFail"} };
        var r= DB.Database("MigrateTest").RunCommandAsync<BsonDocument>(cmd);
        return r;
    }
}
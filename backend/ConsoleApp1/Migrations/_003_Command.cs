using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Entities;

namespace ConsoleApp1.Migrations;

public class _003_Command : IMigration
{
    public Task UpgradeAsync()
    {
        var cmd = new BsonDocument() { {"create", "CmdColl"} };
        return DB.Database("MigrateTest").RunCommandAsync<BsonDocument>(cmd);
    }
}
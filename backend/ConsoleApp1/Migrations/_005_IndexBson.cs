using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Entities;

namespace ConsoleApp1.Migrations;

public class _005_IndexCommand : IMigration
{
    public Task UpgradeAsync()
    {
        var cmd = new BsonDocument() { {"createIndexes", "CmdColl"}, {"indexes", new BsonArray()
        {
            new BsonDocument()
            {
                {"key", new BsonDocument(){ {"InstanceId", 1}, {"Value", -1} }},
                {"name", "CmdIndx"},
                {"unique", true}
            }
        }} };
        var r= DB.Database("MigrateTest").RunCommandAsync<BsonDocument>(cmd);
        return r;
    }
}
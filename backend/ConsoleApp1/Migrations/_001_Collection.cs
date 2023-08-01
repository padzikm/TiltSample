using MongoDB.Entities;

namespace ConsoleApp1.Migrations;

public class _001_Collection : IMigration
{
    public Task UpgradeAsync()
    {
        return DB.Database("MigrateTest").CreateCollectionAsync(nameof(Perf));
    }
}
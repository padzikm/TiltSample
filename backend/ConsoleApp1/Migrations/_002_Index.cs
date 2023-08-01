using MongoDB.Driver;
using MongoDB.Entities;

namespace ConsoleApp1.Migrations;

public class _002_Index : IMigration
{
    public Task UpgradeAsync()
    {
        return DB.Database("MigrateTest").GetCollection<Perf>(nameof(Perf)).Indexes.CreateOneAsync(
            new CreateIndexModel<Perf>(
                new IndexKeysDefinitionBuilder<Perf>().Combine(
                    new IndexKeysDefinition<Perf>[]
                    {
                        new IndexKeysDefinitionBuilder<Perf>().Ascending(p => p.InstanceId),
                        new IndexKeysDefinitionBuilder<Perf>().Descending(p => p.Value)
                    })));
    }
}
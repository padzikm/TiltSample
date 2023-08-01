using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConsoleApp1;

public class Perf
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; }
    
    public int Value { get; set; }
    
    public string InstanceId { get; set; }
}
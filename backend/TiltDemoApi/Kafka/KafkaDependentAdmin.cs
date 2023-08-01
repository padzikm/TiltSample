using Confluent.Kafka;

namespace TiltDemoApi.Kafka;

public class KafkaDependentAdmin
{
    public KafkaDependentAdmin(KafkaClientHandle handle)
    {
        Client = new DependentAdminClientBuilder(handle.Handle).Build();
    }
    
    public IAdminClient Client { get; }
}
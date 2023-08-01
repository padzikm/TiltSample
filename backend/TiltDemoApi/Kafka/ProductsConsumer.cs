using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TiltDemoApi.Xstore;

namespace TiltDemoApi.Kafka
{
    public class ProductsConsumer : BackgroundService
    {
        private readonly string topic;
        private readonly IConsumer<int, Product> kafkaConsumer;

        public ProductsConsumer(IConfiguration config, ISchemaRegistryClient schemaRegistryClient)
        {
            var consumerConfig = new ConsumerConfig(){AutoOffsetReset = AutoOffsetReset.Earliest};
            config.GetSection("Kafka:ConsumerSettings").Bind(consumerConfig);
            this.topic = "products";//config.GetValue<string>("Kafka:RequestTimeTopic");
            this.kafkaConsumer = new ConsumerBuilder<int, Product>(consumerConfig)
                .SetValueDeserializer(new JsonDeserializer<Product>().AsSyncOverAsync()).Build();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
        }
        
        private void StartConsumerLoop(CancellationToken cancellationToken)
        {
            kafkaConsumer.Subscribe(this.topic);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = this.kafkaConsumer.Consume(cancellationToken);

                    // Handle message...
                    Console.WriteLine($"Product {cr.Message.Key}: {cr.Message.Value.Name}");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException e)
                {
                    // Consumer errors should generally be ignored (or logged) unless fatal.
                    Console.WriteLine($"Products consume error: {e.Error.Reason}");

                    if (e.Error.IsFatal)
                    {
                        // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected error: {e}");
                    break;
                }
            }
        }
        
        public override void Dispose()
        {
            this.kafkaConsumer.Close(); // Commit offsets and leave the group cleanly.
            this.kafkaConsumer.Dispose();

            base.Dispose();
        }
    }
}
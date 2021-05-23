using System;
using System.Threading;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using schneider.poem.models;

namespace TagValuePublisher
{
    class Program
    {
        private const string DefaultBootstrapServers = "localhost";
        private const string DefaultSchemaRegistryUrl = "http://localhost:8081";
        private const string DefaultTopicName = "tag-values";
        private const string DefaultIntervalInSeconds = "1";
        private const int MillisecondsInOneSecond = 1000;

        private static readonly string BootstrapServers =
            Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS") ?? DefaultBootstrapServers;
        private static readonly string SchemaRegistryUrl =
            Environment.GetEnvironmentVariable("KAFKA_SCHEMA_REGISTRY_URL") ?? DefaultSchemaRegistryUrl;
        private static readonly string TopicName =
            Environment.GetEnvironmentVariable("KAFKA_TOPIC_NAME") ?? DefaultTopicName;
        private static readonly int IntervalInSeconds = 
            int.Parse(Environment.GetEnvironmentVariable("INTERVAL") ?? DefaultIntervalInSeconds);

        static void Main(string[] args)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = BootstrapServers
            };

            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = SchemaRegistryUrl
            };

            var avroSerializerConfig = new AvroSerializerConfig {BufferBytes = 100};

            var rnd = new Random();

            using (var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig))
            using (var producer =
                new ProducerBuilder<string, TagNumericValue>(producerConfig)
                    .SetKeySerializer(new AvroSerializer<string>(schemaRegistry, avroSerializerConfig))
                    .SetValueSerializer(new AvroSerializer<TagNumericValue>(schemaRegistry, avroSerializerConfig))
                    .Build())
            {
                Console.WriteLine($"Producer [{producer.Name}] producing on topic [{TopicName}] in every [{IntervalInSeconds}] seconds.");
                while (true)
                {
                    var tagValue = new TagNumericValue
                    {
                        tagName = "DummyTagName", 
                        tagValue = rnd.NextDouble(),
                        timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    };
                    producer
                        .ProduceAsync(TopicName, new Message<string, TagNumericValue>
                        {
                            Key = tagValue.tagName, 
                            Value = tagValue
                        })
                        .ContinueWith(task =>
                        {
                            Console.WriteLine(!task.IsFaulted
                                ? $"produced to: {task.Result.TopicPartitionOffset}"
                                : $"error producing message: {task.Exception}");
                        });
                    Thread.Sleep(IntervalInSeconds * MillisecondsInOneSecond);
                }
            }
        }
    }
}
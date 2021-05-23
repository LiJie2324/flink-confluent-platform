```bash
./download_lib.sh
docker-compose up -d
docker exec -it taskmanager sql-client.sh
```

```sql
CREATE TABLE tag_values (
  the_kafka_key STRING,
  tagName STRING,
  tagValue DOUBLE,
  measureTime TIMESTAMP(3)
) WITH (
  'connector' = 'kafka',
  'topic' = 'tag-values',
  'properties.bootstrap.servers' = 'broker:29092',

  'key.format' = 'raw',
  'key.fields' = 'the_kafka_key',

  'value.format' = 'avro-confluent',
  'value.avro-confluent.schema-registry.url' = 'http://schema-registry:8081',
  'value.fields-include' = 'EXCEPT_KEY'
);
SELECT * FROM tag_values;
```
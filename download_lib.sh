#!/bin/bash

if [[ ! -f flink-sql-avro-confluent-registry-1.13.0.jar ]]; then
  curl -OL https://repo1.maven.org/maven2/org/apache/flink/flink-sql-avro-confluent-registry/1.13.0/flink-sql-avro-confluent-registry-1.13.0.jar
fi

if [[ ! -f flink-sql-connector-kafka_2.12-1.13.0.jar ]]; then
  curl -OL https://repo1.maven.org/maven2/org/apache/flink/flink-sql-connector-kafka_2.12/1.13.0/flink-sql-connector-kafka_2.12-1.13.0.jar
fi 
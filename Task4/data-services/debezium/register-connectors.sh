#!/bin/bash

echo "Waiting for Kafka Connect..."

until curl -s http://localhost:8083/connectors; do
  sleep 3
done

echo "Registering Debezium connector..."

curl -X POST \
  -H "Content-Type: application/json" \
  --data @/kafka/connectors/crm-connector.json \
  http://localhost:8083/connectors

echo "Connector registered"
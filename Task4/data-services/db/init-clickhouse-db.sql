-- stg_clients
CREATE TABLE stg_clients
(
    client_id UInt32,
    full_name String,
    birth_date Date,
    city String,
    updated_at DateTime
)
ENGINE = ReplacingMergeTree(updated_at)
ORDER BY client_id;

--stg_telemetry
CREATE TABLE stg_telemetry
(
    client_id UInt32,
    event_time DateTime,
    steps UInt32,
    battery_level UInt8,
    loaded_at DateTime
)
ENGINE = ReplacingMergeTree(loaded_at)
ORDER BY (client_id, event_time);

--dm_clients_telemetry (Data Mart)
CREATE TABLE dm_clients_telemetry
(
    client_id UInt32,
    full_name String,
    city String,
    total_steps UInt64,
    avg_battery Float32,
    last_activity DateTime,
    calculated_at DateTime
)
ENGINE = ReplacingMergeTree(calculated_at)
ORDER BY client_id;

CREATE TABLE kafka_clients
(
    payload String
)
ENGINE = Kafka
SETTINGS
    kafka_broker_list = 'kafka:9092',
    kafka_topic_list = 'crm.public.clients',
    kafka_group_name = 'clickhouse',
    kafka_format = 'JSONEachRow';

CREATE MATERIALIZED VIEW mv_clients
TO stg_clients
AS
SELECT
    JSONExtractUInt(payload, 'payload.after.client_id') as client_id,
    JSONExtractString(payload, 'payload.after.full_name') as full_name,
    toDate(JSONExtractString(payload, 'payload.after.birth_date')) as birth_date,
    JSONExtractString(payload, 'payload.after.city') as city,
    now() as updated_at
FROM kafka_clients
WHERE JSONExtractString(payload, 'payload.op') IN ('c','u');


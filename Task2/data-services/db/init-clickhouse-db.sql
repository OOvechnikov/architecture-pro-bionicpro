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

--CREATE USER IF NOT EXISTS airflow IDENTIFIED WITH plaintext_password BY 'airflow';
--GRANT ALL ON *.* TO airflow;
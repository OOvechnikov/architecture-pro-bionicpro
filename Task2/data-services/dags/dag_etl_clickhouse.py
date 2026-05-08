from airflow import DAG
from airflow.operators.python import PythonOperator
from datetime import datetime
import pandas as pd
from clickhouse_driver import Client

CLICKHOUSE_HOST = "clickhouse"

def load_clients():
    df = pd.read_csv("/opt/airflow/sample_files/crm-clients-data.csv")

    # Приводим birth_date к date
    df["birth_date"] = pd.to_datetime(df["birth_date"]).dt.date
    df["updated_at"] = datetime.utcnow()

    client = Client(
        host=CLICKHOUSE_HOST,
        user="airflow",
        password="airflow",
        settings={
            'async_insert': 0,  # Отключаем асинхронные вставки
            'wait_for_async_insert': 0
        }
    )

    # Преобразуем DataFrame в список списков для ClickHouse
    data = df[['client_id', 'full_name', 'birth_date', 'city', 'updated_at']].values.tolist()

    client.execute(
        """
        INSERT INTO stg_clients
        (client_id, full_name, birth_date, city, updated_at)
        VALUES
        """,
        data
    )

    print(f"Inserted {len(data)} rows into stg_clients")

def load_telemetry():
    df = pd.read_csv("/opt/airflow/sample_files/clients-telemetry-data.csv")

    # Преобразуем строку в datetime
    df["event_time"] = pd.to_datetime(df["event_time"])
    df["loaded_at"] = datetime.utcnow()

    client = Client(
        host=CLICKHOUSE_HOST,
        user="airflow",
        password="airflow",
        settings={
            'async_insert': 0,
            'wait_for_async_insert': 0
        }
    )

    # Преобразуем в список списков
    data = df[['client_id', 'event_time', 'steps', 'battery_level', 'loaded_at']].values.tolist()

    client.execute(
        """
        INSERT INTO stg_telemetry
        (client_id, event_time, steps, battery_level, loaded_at)
        VALUES
        """,
        data
    )

    print(f"Inserted {len(data)} rows into stg_telemetry")

def build_mart():
    client = Client(
        host=CLICKHOUSE_HOST,
        user="airflow",
        password="airflow"
    )

    # Сначала очистим витрину (если нужно обновлять, а не добавлять)
    client.execute("TRUNCATE TABLE IF EXISTS dm_clients_telemetry")

    # Исправленный запрос без FINAL в JOIN
    query = """
    INSERT INTO dm_clients_telemetry
    WITH
        latest_clients AS (
            SELECT
                client_id,
                argMax(full_name, updated_at) as full_name,
                argMax(city, updated_at) as city
            FROM stg_clients
            GROUP BY client_id
        ),
        latest_telemetry AS (
            SELECT
                client_id,
                event_time,
                steps,
                battery_level
            FROM stg_telemetry
        )
    SELECT
        c.client_id,
        c.full_name,
        c.city,
        sum(t.steps) as total_steps,
        avg(t.battery_level) as avg_battery,
        max(t.event_time) as last_activity,
        now() as calculated_at
    FROM latest_clients c
    INNER JOIN latest_telemetry t ON c.client_id = t.client_id
    GROUP BY
        c.client_id,
        c.full_name,
        c.city
    """

    result = client.execute(query)
    print(f"Built data mart successfully")

with DAG(
    dag_id="etl_clickhouse_pipeline",
    start_date=datetime(2024, 1, 1),
    schedule_interval="@daily",
    catchup=False,
    tags=["etl", "clickhouse"],
) as dag:

    t1 = PythonOperator(
        task_id="load_clients",
        python_callable=load_clients
    )

    t2 = PythonOperator(
        task_id="load_telemetry",
        python_callable=load_telemetry
    )

    t3 = PythonOperator(
        task_id="build_data_mart",
        python_callable=build_mart
    )

    [t1, t2] >> t3
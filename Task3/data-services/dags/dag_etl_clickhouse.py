from airflow import DAG
from airflow.operators.python import PythonOperator
from datetime import datetime
import pandas as pd
from clickhouse_driver import Client

CLICKHOUSE_HOST = "clickhouse"

def load_clients():
    df = pd.read_csv("/opt/airflow/sample_files/crm-clients-data.csv")

    # 👇 ВАЖНО: приводим birth_date к date
    df["birth_date"] = pd.to_datetime(df["birth_date"]).dt.date

    # version колонка
    df["updated_at"] = datetime.utcnow()

    client = Client(
        host=CLICKHOUSE_HOST,
        user="airflow",
        password="airflow"
    )

    client.execute(
        """
        INSERT INTO stg_clients
        (client_id, full_name, birth_date, city, updated_at)
        VALUES
        """,
        df.to_dict("records")
    )

def load_telemetry():
    df = pd.read_csv("/opt/airflow/sample_files/clients-telemetry-data.csv")

    # 👇 ВАЖНО: преобразуем строку в datetime
    df["event_time"] = pd.to_datetime(df["event_time"])

    # version колонка
    df["loaded_at"] = datetime.utcnow()

    client = Client(
        host=CLICKHOUSE_HOST,
        user="airflow",
        password="airflow"
    )

    client.execute(
        """
        INSERT INTO stg_telemetry
        (client_id, event_time, steps, battery_level, loaded_at)
        VALUES
        """,
        df.to_dict("records")
    )

def build_mart():
    client = Client(
        host=CLICKHOUSE_HOST,
        user="airflow",
        password="airflow"
    )

    query = """
    INSERT INTO dm_clients_telemetry
    SELECT
        c.client_id,
        c.full_name,
        c.city,
        sum(t.steps) as total_steps,
        avg(t.battery_level) as avg_battery,
        max(t.event_time) as last_activity,
        now() as calculated_at
    FROM
        (SELECT * FROM stg_clients FINAL) AS c
    JOIN
        (SELECT * FROM stg_telemetry FINAL) AS t
    ON c.client_id = t.client_id
    GROUP BY
        c.client_id,
        c.full_name,
        c.city
    """

    client.execute(query)


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
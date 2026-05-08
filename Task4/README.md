# Задание 4. Повышение оперативности и стабильности работы CRM

## Решение

1. В [docker-compose](./docker-compose.yml) добавлены компоненты
    - crm-db (PostgresDB для CRM)
    - zookeeper
    - kafka
    - kafka-connect

2. Из [data-services/dags/dag_etl_clickhouse.py](./data-services/dags/dag_etl_clickhouse.py) удален шаг загрузки данных из CRM (т.к. теперь данные будут проливаться через debezium)

3. Добавлены sql-скрипты [init-clickhouse-db.sql](./data-services/db/init-clickhouse-db.sql) для создания `kafka_clients` и MATERIALIZED VIEW `mv_clients`

4. Добавлена конфигурация для активации Debezium - [debezium/crm-connector.json](./data-services/debezium/crm-connector.json)
  - Она может быть применена на старте контейнера (закомментировано)
  - Так же можно применить командой:
```bash
curl -X POST \
  -H "Content-Type: application/json" \
  --data @./data-services/debezium/crm-connector.json \
  http://127.0.0.1:8083/connectors
```

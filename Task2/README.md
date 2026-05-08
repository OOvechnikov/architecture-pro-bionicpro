# Задание 2. Разработка сервиса отчётов

## Запуск

```bash
cd /Task2/

docker compose up --build
```

## 1. Архитектура решения

[BionicPRO_C4_model.drawio.xml](./BionicPRO_C4_model.drawio.xml)

## 2. Airflow DAG

За основу взят [пример из теории](https://github.com/Yandex-Practicum/architecture-DWH-pipeline).

### Архитектруа текущего примера работы с Airflow

- Источники данных:
    - [crm-clients-data.csv](./data-services/data/crm-clients-data.csv)
    - [clients-telemetry-data.csv](./data-services/data/clients-telemetry-data.csv)
- OLAP: ClickHouse
- Оркестратор: Apache Airflow
- Executor: LocalExecutor
- DAG: по расписанию

### DAG Pipeline

1. Extract
    - Читаем CSV
    - Load (staging)
2. Загружаем:
    - stg_clients
    - stg_telemetry
3. Transform
    - Агрегируем телеметрию
    - JOIN с клиентами
4. Load (Data Mart)
    - Пишем в dm_clients_telemetry

### Запуск

3. Открыть http://localhost:8085 

4. Включить DAG
    - Найти etl_clickhouse_pipeline
    - Включить переключатель
    - Нажать Trigger

### Проверка

1. Открыть Clickhouse play ( http://localhost:8123/play)
2. Выполнить
```sql
SELECT * FROM dm_clients_telemetry FINAL;
```

![airflowDag_1.png](./airflowDag_1.png)

## 3. Report service Backend Api

[ReportService.sln](./data-services/report-service/ReportService.sln) сервис реализован.

Проверка:
GET [http://localhost:8086/api/reports?clientId=user1](http://localhost:8086/api/reports?clientId=user1)

![reports-api-response.png](./reports-api-response.png)

## 4. Получение отчета через Frontend

1. В сервис [/Task2/auth-services/bionicpro-auth/BionicProAuth.sln](./auth-services/bionicpro-auth/BionicProAuth.sln) добавлен метод проксирования вызова отчета по телеметрии в [ReportService.sln](/Task2/data-services/report-service/ReportService.sln).
2. В сервис [frontend - ReportPage.tsx](./auth-services/frontend/src/components/ReportPage.tsx) добавлен вызов бекенда для получения отчета.
3. Вызов в [api/v1/reports](./auth-services/bionicpro-auth/Controllers/ReportsController.cs) выполняется без передачи параметров. Определение clientId (userId) происходит посредством извлечения данных из авторизованной сессии клиента. Отчет запрашивается из [ReportService.sln](/Task2/data-services/report-service/ReportService.sln) только в случае успешного прохождения авторизации.

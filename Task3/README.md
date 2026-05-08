# Задание 3. Снижение нагрузки на базу данных

## Решение

### 1. S3 хранилище

В [docker-compose](./docker-compose.yml) добавлена конфигурация развертывания minio. В нем будут храниться ранее сгенерированные отчеты.

Так же добавлен init-контейнер, который установит политику автоудаления отчетов, которые старше одного дня - [lifecycle.json](./data-services/minio/lifecycle.json)

### 2. Report-service

Доработан [ReportService](./data-services/report-service/Services/ClientReportService.cs) - теперь он сначала проверяет наличие отчета в S3 (minio)
- если отчет есть в S3 - он отдается сразу.
- если отчета нет в S3 - он запрашивается из ClickHouse, сохраняется в S3 и отдается после этого.
- время жизни кэша в S3 - 1 день

### 3. CDN cache

- Добавлен контейнер nginx, который реализует HTTP cache - [nginx.conf](./data-services/nginx/nginx.conf)
- Такой кэш краткосрочный - истекает через 10 минут.
- В BionicProAuth, для запроса отчета, ссылка заменена на адрес nginx сервиса ["Reports.Url": "http://nginx:80"](./auth-services/bionicpro-auth/appsettings.json)

### 4. Проверка

1. CDN кэша в nginx нет, отчет запрашивается из report-service. Отчет запрашивался меньше чем день назад, потому он есть в S3:
![screen1](./user-report1.png)

2. Отчет уже закэширован в nginx, отдается сразу, запроса в report-service нет
![screen2](./user-report2.png)

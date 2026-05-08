# Задача 6. Добавьте OAuth 2.0 от Яндекс ID

## Настройка

### 1. Регистрация приложения в Яндекс ID
   - Открыть консоль разработчика: https://oauth.yandex.ru/
   - Создайте первое приложение - Для авторизации пользователей
   - Тип: Web service
   - Redirect URI: http://localhost:8082/auth/yandex/callback
   - Права доступа (Scopes):
      - login:email
      - login:info

При создании будут получены:
   - Client ID
   - Client Secret
   
Данные сохранены в [bionicpro-auth/appsettings.json](/Task1/Task1-6/bionicpro-auth/appsettings.json)

### 2. OAuth flow

Код интеграции с Yandex реализован в бекенд-приложении [bionicpro-auth/BionicProAuth.sln](/Task1/Task1-6/bionicpro-auth/BionicProAuth.sln)
   - Реализован /auth/yandex/login
   - Реализован /auth/yandex/callback
   - Рeализовано хранение пользователей в БД (пока это redis)

## Решение

#### 1.Создать Web OAuth приложение в Яндекс ID

![screen1](/Task1/Task1-6/Screenshot_ya1.png)

#### 2.При авторизации можно войти по логину-паролю, и можно через Yandex

![screen2](/Task1/Task1-6/Screenshot_ya2.png)

#### 3.При входе предлагается выбрать учетную запись

![screen3](/Task1/Task1-6/Screenshot_ya3.png)

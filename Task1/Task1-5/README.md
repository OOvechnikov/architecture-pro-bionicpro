# Задача 5. Настройка MFA

## Настройка

1. Зайти в админку ( http://localhost:8080 )
   - Realm - reports-realm

2. Сделать OTP обязательным
   - Authentication - вкладка Required Actions - Configure OTP
        - Enabled - true
        - Set as default action - true


## Проверка

1. Пользователь логинится через LDAP
2. Если OTP ещё не настроен - Keycloak попросит отсканировать QR

   ![screen1](/Task1/Task1-5/Screenshot_mfa1.png)

3. После настройки OTP — каждый вход требует код

![screen1](/Task1/Task1-5/Screenshot_mfa2.png)


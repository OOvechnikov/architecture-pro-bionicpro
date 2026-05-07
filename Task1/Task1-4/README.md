# Задача 4. Добавьте LDAP для возможности получения данных о пользователях представительства BionicPRO в другой стране

## Настройка

Подключить базу LDAP:
```bash
docker exec -it openldap ldapadd -x \
  -D "cn=admin,dc=example,dc=com" \
  -w admin \
  -f /tmp/config.ldif

```

Проверить OpenLDAP:
```bash
ldapsearch -x -H ldap://localhost:389 -D "cn=admin,dc=example,dc=com" -w admin -b "dc=example,dc=com"
```

### Добавить LDAP провайдер
- В меню слева выбрать User Federation
- Нажать Add provider - выбрать ldap
- Настроить параметры подключения:
 - Bind Credential	- admin
 - User Object Classes	inetOrgPerson
 - Import Users	- true
 - Save

Теперь Keycloak видит LDAP и может импортировать пользователей.

### Добавить LDAP mappers для групп и ролей
- Открыть User Federation - выбрать LDAP провайдер - вкладка Mappers
- Нажать Create - выбрать Mapper Type: group-ldap-mapper

Настроить mapper:
- Name	groups-mapper
- LDAP Groups DN	ou=Groups,dc=example,dc=com
- Group Name LDAP Attribute	cn
- Group Object Classes	groupOfNames
- Membership LDAP Attribute	member
- Membership Attribute Type	DN
- Mode	READ_ONLY
- User Roles Retrieve Strategy	LOAD_GROUPS_BY_MEMBER_ATTRIBUTE
- Groups Path	/
- Preserve Group Inheritance	true
- Ignore Missing Groups	false
- Drop Non Existing Groups During Sync	false
- Save

### Синхронизация LDAP
- В User Federation - LDAP провайдер → Sync Settings
- Можно запустить Full Sync или Changed Users Sync вручную

## Результат

После синхронизации пользователи получат роли из групп LDAP:
![screen1](/Task1/Task1-4/syncScreen.png)

![screen2](/Task1/Task1-4/syncScreen2.png)
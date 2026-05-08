#!/bin/bash
set -e

# Keycloak параметры
KC_URL="http://127.0.0.1:8080"
REALM="reports-realm"
ADMIN_USER="admin"
ADMIN_PASS="admin"
LDAP_PROVIDER_ID="ldap-provider"

# 1. Авторизация
/opt/keycloak/bin/kcadm.sh config credentials --server $KC_URL --realm master --user $ADMIN_USER --password $ADMIN_PASS

# /opt/keycloak/bin/kcadm.sh config credentials --server "http://127.0.0.1:8080" --realm master --user "admin" --password "admin"

# 2. Создать mapper для групп LDAP → realm roles
/opt/keycloak/bin/kcadm.sh create components -r "reports-realm" -s name=groups-mapper \
  -s providerId=group-ldap-mapper \
  -s parentId="ldap-provider" \
  -s "subType=ldap-group-Mapper" \
  -s providerType=org.keycloak.storage.ldap.mappers.LDAPStorageMapper \
  -s config.ldapGroupsDn=ou=Groups,dc=example,dc=com \
  -s config.groupNameLDAPAttribute=cn \
  -s config.groupObjectClasses=groupOfNames \
  -s config.membershipLDAPAttribute=member \
  -s config.membershipAttributeType=DN \
  -s config.mode=READ_ONLY \
  -s config.userRolesRetrieveStrategy=LOAD_GROUPS_BY_MEMBER_ATTRIBUTE \
  -s config.groupsPath=/ \
  -s config.preserveGroupInheritance=true \
  -s config.ignoreMissingGroups=false \
  -s config.dropNonExistingGroupsDuringSync=false

# 3. Опционально: синхронизировать группы
/opt/keycloak/bin/kcadm.sh update components/$LDAP_PROVIDER_ID -r $REALM -s "config.syncRegistrations=true"

echo "LDAP mappers созданы и синхронизированы."

# Application Backend

## BrimSchedule.API
Входная для сборки и конечная для пользователя точка приложения. Содержит контроллеры, дтошки, конфиг web-приложения.

## BrimSchedule.Application
Слой бизнес-логики приложения

## BrimSchedule.Domain
Слой для хранения доменных моделек предметной области

## BrimSchedule.Infrastructure
Слой для взаимодействия с внешними сервисами

## BrimSchedule.Persistence
Слой для взаимодействия с БД. Здесь стоит хранить контекст EF, реализацию репозиториев - в целом все, что касается взаимодействия с хранилищем данных.

# Настройка локальной среды

## БД разработки

Для тестирования необходимо установить PostgreSQL со всеми его компонентами:

- PostgreSQL server (при установке указываем пароль админа БД 12345) https://www.enterprisedb.com/downloads/postgres-postgresql-downloads
- Npgsql драйвер (в конце установки PostgreSQL предложит запустить инсталлятор Stack Builder, соглашаемся, в нем разворачиваем пункт Database Drivers ставим чекбокс у Npgsql)
- веб-интерфейс для работы с БД (dBeaver или pgAdmin)
https://dbeaver.io/
https://www.pgadmin.org/download/pgadmin-4-windows/

### Миграции

Для управления миграциями из командной строки, необходимо установить тулзу командой

dotnet tool install --global dotnet-ef

Миграции в проекте настроены автоматически. Чтобы создать новую миграцию, необходимо перейти в каталог Persistence и выполнить команду

dotnet ef migrations add <название_миграции>
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

Для управления миграциями из командной строки, необходимо установить EF CLI tools следующей командой:

dotnet tool install --global dotnet-ef

Далее в комадной строке переходим в каталог BrimSchedule.Persistence, откуда будет осуществляться все последующее управление миграциями.

Миграции в проекте настроены автоматически. Чтобы создать новую миграцию, необходимо выполнить команду:

dotnet ef --startup-project ../BrimSchedule.API/ migrations add <название_миграции>

Все миграции существуют в виде С#-классов в каталоге BrimSchedule.Persistence/Migrations. Помимо миграций, в каталоге существует Snapshot-класс текущей базы.

Чтобы посмотреть SQL скрипт, который будет сгенерирован миграциями, можно воспользоваться командой:

dotnet ef --startup-project ../BrimSchedule.API/ migrations script

Для удаления миграций можно выполнить команду:

dotnet ef --startup-project ../BrimSchedule.API/ migrations remove

Если миграции не накатываются, то с помощью следующих команд можно пересоздать БД. Для этого необходимо перейти в каталог BrimSchedule.API в терминале:

dotnet ef --startup-project ../BrimSchedule.API/ database drop
dotnet ef --startup-project ../BrimSchedule.API/ database update
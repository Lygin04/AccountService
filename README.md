## Описание выполненного задания

### Запуск приложения
Приложение запускается командой `docker-compose -f compose.yaml up -d`. Вместе с основным сервисом поднимается Keycloak.
Приложение запускается на порту `8080`. Для тестирования доступен swagger по 
[ссылке](http://localhost:8080/swagger/index.html)

### Документация API
#### /v1/Accounts
- POST Создает счет
- PATCH /{accountId:guid} изменяет счет по идентификатору
- DELETE /{accountId:guid} удаляет счет по идентификатору
- GET /{accountId:guid} возвращает счет по идентификатору 
- GET /owner/{ownerid:guid} Получает выписку всех счетов по идентификатору владельца
- POST /transfer Переводит средства между счетами

#### /v1/Transactions
- POST Создает транзакцию из внешнего сервиса
- GET /{accountId:guid Получает список транзакций по идентификатору счета

#### /health
- [live](http://localhost:8080/health/live) Просто проверяет, что приложение работает (без зависимостей).
- [ready](http://localhost:8080/health/ready) Проверяет соединения с Postgres и RabbitMQ.

### Инструкция использования Keycloak в Swagger
При нажатии кнопки `Authorize` в появившемся окне нужно заполнить поле client_id: myclient и выбрать Scopes: select all
после чего еще раз нажать кнопку `Authorize`. Далее нас перебрасывает на страницу аутентификации где нужно заполнить 
поля `Username or email: testuser` `Password: testpass` и нажать кнопку sign in. На этом аутентификации завершена и мы 
можем вызывать все конечные точки с атрибутом Authorize.

### Инструкция запуска тестов
Перед началом тестов нужно ОБЯЗАТЕЛЬНО снести рабочий контейнер командой `docker-compose down -v` запустить контейнер 
с тестовой бд и RabbitMQ.\
Вот команда для запуска тестового контейнера `docker-compose -f compose.override.yaml up -d` после закпуска подождите 
30с чтобы бд и rabbitMQ успели запуститься.\
Тесты запускаются командой `dotnet test; docker-compose -f compose.override.yaml down -v`.
После выполнения тестов контейнер сам удалиться.

### Инструкци по открытию дашбордов
##### HangFire
[ссылка на дашборд](http://localhost:8080/hangfire)

##### RabbitMQ
[ссылка на дашборд](http://localhost:15672) \
Username: guest \
Password: guest

### Логгирование
Логирование выполнено по ТЗ, с сохранением в файле который находится в корне проекта AccountService в папке logs.
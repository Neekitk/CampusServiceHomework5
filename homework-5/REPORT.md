# Практическая работа №5

## Проектирование безопасных релизных процессов, паттернов миграции данных и эксплуатационной документации ИС «Умный кампус»

**ФИО:** Корнышев Н.А.  
**Группа:** И13-24-11  
**Вариант:** 7  
**Модуль:** Заказ справок (Деканат)  
**Задача:** `CAMPUS-107`  
**Изменение:** `Status` → `StatusCode` (`enum/string` → `int`)

---

## 1. Единый цифровой след (Digital Traceability)

Для задачи `CAMPUS-107` в контроллер модуля заказа справок добавлены OpenTelemetry-теги через `Activity.Current`.

Используются следующие теги:

```csharp
Activity.Current?.SetTag("jira.issue", "CAMPUS-107");
Activity.Current?.SetTag("campus.request_id", request.Id);
Activity.Current?.SetTag("campus.certificate_type", request.CertificateType);
```

Таким образом формируется сквозная цепочка:

```text
Jira CAMPUS-107
    ↓
Git Commit
    ↓
CertificateRequestsController.cs
    ↓
Activity.Current
    ↓
OpenTelemetry Trace
```

Git-коммит:

```text
[CAMPUS-107] Add tracing tags and business logic
```

**Файл контроллера:**  
[`Controllers/CertificateRequestsController.cs`](homework-5/Controllers/CertificateRequestsController.cs)

---

## 2. Регламент релиза и Change Management

Для релиза версии `v1.9.0` подготовлен Change Request.

В документе определены:

- описание релиза и бизнес-цель;
- окно обслуживания;
- план проведения работ;
- условия запуска отката;
- команда отката Kubernetes;
- правила Branch Protection для ветки `main`.

Выбранное окно обслуживания:

**Воскресенье, 02:00–03:00 МСК.**

Для отката используется команда:

```bash
kubectl rollout undo deployment/campus-service -n production
```

Для ветки `main` определены следующие правила:

- прямые изменения запрещены;
- изменения принимаются только через Pull Request;
- требуется минимум 2 одобрения;
- требуется одобрение Tech Lead;
- CI/CD-проверки и автоматические тесты должны быть успешными;
- Force Push запрещён;
- удаление защищённой ветки запрещено.

**Change Request:**  
[`docs/CHANGE_REQUEST.md`](homework-5/docs/CHANGE_REQUEST.md)

---

## 3. Безопасная миграция базы данных

Для перехода от поля `Status` к `StatusCode` используется паттерн **Expand and Contract**.

### Фаза 1 — Expand

Добавляется новое поле `StatusCode`, при этом старое поле `Status` продолжает существовать.

**SQL-скрипт:**  
[`Migrations/01_expand.sql`](homework-5/Migrations/01_expand.sql)

На переходном этапе приложение записывает данные одновременно в два поля:

```csharp
request.Status = "Created";
request.StatusCode = 1;
```

### Фаза 2 — Migrate

Исторические значения поля `Status` преобразуются в числовые значения и переносятся в `StatusCode`.

Применяется следующая схема:

```text
Created    → 1
InProgress → 2
Completed  → 3
Rejected   → 4
```

**SQL-скрипт:**  
[`Migrations/02_migrate.sql`](homework-5/Migrations/02_migrate.sql)

### Фаза 3 — Contract

После успешного перехода всех компонентов системы на новое поле старое поле `Status` удаляется.

**SQL-скрипт:**  
[`Migrations/03_contract.sql`](homework-5/Migrations/03_contract.sql)

Применение паттерна Expand and Contract позволяет выполнить изменение структуры базы данных без остановки приложения и сохранить обратную совместимость на переходном этапе.

---

## 4. Sanity-тестирование и Docs-as-Code

### Production Sanity-тест

Для проверки работоспособности модуля после релиза разработан xUnit-тест:

```text
Sanity_CreateAndDeleteCertificateRequest
```

Тест выполняет следующие действия:

1. Создаёт тестовую заявку на справку.
2. Проверяет успешное создание заявки.
3. Выполняет Cleanup через DELETE-запрос.
4. Проверяет успешное удаление тестовых данных.

Такой подход позволяет проверить основной рабочий сценарий и не оставлять тестовые данные после завершения проверки.

**Файл теста:**  
[`Tests/ProductionSanityTests.cs`](homework-5/Tests/ProductionSanityTests.cs)

### ADR

Для фиксации архитектурного решения подготовлен документ ADR 0009.

В нём обоснован выбор паттерна Expand and Contract для безопасного перехода от `Status` к `StatusCode`.

**ADR:**  
[`docs/adr/0009-certificate-status-code.md`](homework-5/docs/adr/0009-certificate-status-code.md)

### Changelog

Изменения релиза `v1.9.0` зафиксированы в `CHANGELOG.md`.

В журнале описаны:

- добавленные OpenTelemetry-теги;
- новое поле `StatusCode`;
- Production Sanity-тест;
- изменения, связанные с переходом на новое поле статуса;
- Breaking Change в виде последующего удаления поля `Status`.

**Changelog:**  
[`CHANGELOG.md`](homework-5/CHANGELOG.md)

---

## Результат работы

В рамках практической работы был реализован единый цифровой след для задачи `CAMPUS-107`, подготовлен регламент безопасного релиза версии `v1.9.0`, разработана миграция базы данных по паттерну Expand and Contract и создан Production Sanity-тест с обязательной очисткой тестовых данных.

Также подготовлена эксплуатационная документация в формате Docs-as-Code: Change Request, ADR и Changelog.

## Заключение

Выполненная работа позволила отработать основные этапы безопасного внедрения изменений в информационную систему. Использование OpenTelemetry обеспечивает связь бизнес-задачи с выполняемыми операциями приложения, а правила Branch Protection и Change Request снижают вероятность неконтролируемых изменений в основной ветке.

Паттерн Expand and Contract позволяет изменять структуру базы данных поэтапно, сохраняя работоспособность существующих клиентов. Sanity-тест с Cleanup позволяет быстро проверить основной сценарий после релиза без накопления тестовых данных.

В результате были закреплены навыки организации безопасного релизного процесса, миграции данных без простоя и ведения эксплуатационной документации непосредственно в репозитории проекта.

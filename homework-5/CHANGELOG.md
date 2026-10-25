# Changelog

Все значимые изменения проекта фиксируются в данном файле.

## [1.9.0]

### Added

- Добавлены OpenTelemetry-теги для задачи `CAMPUS-107`.
- Добавлен тег `campus.request_id`.
- Добавлен тег `campus.certificate_type`.
- Добавлено новое поле `StatusCode`.
- Добавлен Production Sanity-тест с автоматическим Cleanup.

### Fixed

- Подготовлена безопасная обработка статусов заявок в период миграции.
- Добавлена совместная запись данных в `Status` и `StatusCode`.

### Breaking Changes

- После завершения фазы Contract поле `Status` будет удалено.
- Клиенты должны перейти на использование поля `StatusCode`.

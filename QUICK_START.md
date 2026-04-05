# ⚡ Быстрый старт — Город Денег

## Требования

| Инструмент | Версия | Скачать |
|-----------|--------|---------|
| Visual Studio 2022 | 17.x+ | [visualstudio.microsoft.com](https://visualstudio.microsoft.com/) |
| .NET SDK | 8.0+ | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) |
| SQL Server | 2022 или LocalDB | [microsoft.com/sql-server](https://www.microsoft.com/sql-server) |
| Node.js | 18+ | [nodejs.org](https://nodejs.org/) |

> **LocalDB** (входит в Visual Studio) — самый простой вариант для разработки, ничего дополнительно устанавливать не нужно.

---

## Шаг 1 — Открыть Solution в VS 2022

```
Файл → Открыть → Проект/Solution → GorodDeneg.sln
```

---

## Шаг 2 — Настроить строку подключения

Откройте `GorodDeneg.API/appsettings.json`:

**LocalDB (по умолчанию — работает без настройки):**
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GorodDeneg;Trusted_Connection=True;TrustServerCertificate=True;"
```

**SQL Server 2022:**
```json
"DefaultConnection": "Server=localhost;Database=GorodDeneg;Trusted_Connection=True;TrustServerCertificate=True;"
```

**SQL Server с паролем:**
```json
"DefaultConnection": "Server=localhost;Database=GorodDeneg;User Id=sa;Password=ВашПароль;TrustServerCertificate=True;"
```

---

## Шаг 3 — Запустить API

### Вариант A: через Visual Studio
1. Убедитесь, что `GorodDeneg.API` выбран как Startup Project
2. Нажмите **F5** (или кнопку ▶️ с профилем `https`)
3. Swagger откроется автоматически на `https://localhost:7001/swagger`

### Вариант B: через терминал
```bash
cd GorodDeneg.API
dotnet run --launch-profile https
```

> ✅ При первом запуске автоматически:
> - Применяются все миграции EF Core
> - Создаются роли Admin и User
> - Создаются демо-пользователи
> - Загружаются 6 тестовых проектов и 19 категорий

---

## Шаг 4 — Создать миграции (если нужно пересоздать)

В Package Manager Console (VS 2022):
```powershell
Add-Migration InitialCreate -Project GorodDeneg.API
Update-Database -Project GorodDeneg.API
```

Или в терминале:
```bash
cd GorodDeneg.API
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## Шаг 5 — Запустить фронтенд

```bash
cd goroddeneg-frontend
npm install          # первый раз
npm run dev          # запустить dev-сервер
```

Откройте браузер: **http://localhost:5173**

---

## Демо-аккаунты

| Роль | Email | Пароль |
|------|-------|--------|
| 👤 Пользователь | `aibek@goroddeneg.kz` | `User@123456!` |
| ⚙️ Администратор | `admin@goroddeneg.kz` | `Admin@123456!` |

---

## Структура URL

| Адрес | Назначение |
|-------|-----------|
| `http://localhost:5173` | Vue фронтенд |
| `https://localhost:7001/swagger` | Swagger UI |
| `https://localhost:7001/health` | Health check |
| `https://localhost:7001/api/projects` | API пример |

---

## Частые проблемы

### ❌ "Cannot connect to database"
- Убедитесь, что SQL Server / LocalDB запущен
- Проверьте строку подключения в `appsettings.json`
- Для LocalDB: запустите `sqllocaldb start MSSQLLocalDB`

### ❌ CORS ошибки в браузере
- Убедитесь, что API запущен на `https://localhost:7001`
- В `appsettings.json` → `Cors.AllowedOrigins` должен быть `http://localhost:5173`

### ❌ "SSL certificate error" в Vite
- В `vite.config.js` уже установлено `secure: false` для proxy

### ❌ "dotnet ef not found"
```bash
dotnet tool install --global dotnet-ef
```

### ❌ Node modules конфликты
```bash
cd goroddeneg-frontend
rm -rf node_modules package-lock.json
npm install
```

---

## Команды для разработки

```bash
# Бэкенд — добавить миграцию
dotnet ef migrations add ИмяМиграции

# Бэкенд — обновить БД
dotnet ef database update

# Бэкенд — откатить миграцию
dotnet ef database update ПредыдущаяМиграция

# Фронтенд — сборка для продакшна
npm run build

# Фронтенд — предпросмотр сборки
npm run preview
```

---

## Деплой на продакшн

1. Измените `Jwt.Secret` на уникальный секрет (мин. 32 символа)
2. Установите `RequireHttpsMetadata = true` в `Program.cs`
3. Обновите `Cors.AllowedOrigins` на ваш домен
4. Соберите фронтенд: `npm run build` → загрузите папку `dist/` на хостинг
5. Опубликуйте API: `dotnet publish -c Release`

# 🏙️ Город Денег — Краудфандинговая платформа

Полный стек приложение: **ASP.NET Core 8 + ASP.NET Identity + EF Core + SQL Server 2022 + Vue 3 + Vite**

---

## 📁 Структура проекта

```
GorodDeneg/
├── GorodDeneg.sln                          ← Solution для Visual Studio 2022
│
├── GorodDeneg.API/                         ← ASP.NET Core 8 Web API
│   ├── GorodDeneg.API.csproj
│   ├── Program.cs                          ← Startup, DI, Identity, JWT, CORS
│   ├── appsettings.json                    ← Connection string, JWT config
│   ├── Models/
│   │   └── Models.cs                       ← AppUser, Project, Reward, Pledge...
│   ├── Data/
│   │   ├── AppDbContext.cs                 ← EF Core + Identity DbContext
│   │   └── DbSeeder.cs                     ← Roles, demo users, demo projects
│   ├── DTOs/
│   │   └── DTOs.cs                         ← Все DTO (Records и классы)
│   ├── Services/
│   │   ├── TokenService.cs                 ← JWT генерация
│   │   └── MappingProfile.cs               ← AutoMapper
│   └── Controllers/
│       └── Controllers.cs                  ← Auth, Projects, Users, Categories, Admin
│
└── goroddeneg-frontend/                    ← Vue 3 + Vite SPA
    ├── index.html
    ├── package.json
    ├── vite.config.js
    └── src/
        ├── main.js
        ├── App.vue
        ├── router/index.js
        ├── stores/auth.js                  ← Pinia
        ├── services/api.js                 ← Axios API client
        ├── assets/css/main.css             ← Design system (кремово-золотая тема)
        ├── components/
        │   ├── common/
        │   │   ├── AppNav.vue
        │   │   └── AppFooter.vue
        │   └── project/
        │       └── ProjectCard.vue
        └── views/
            ├── HomeView.vue                ← Лендинг
            ├── ProjectsView.vue            ← Каталог с фильтрами
            ├── ProjectDetailView.vue       ← Страница проекта + оплата
            ├── LoginView.vue
            ├── RegisterView.vue
            ├── CreateProjectView.vue       ← 5-шаговый визард
            ├── CabinetView.vue             ← Личный кабинет
            ├── AdminView.vue               ← Панель администратора
            ├── HowItWorksView.vue
            ├── AboutView.vue
            └── WhyView.vue
```

---

## 🚀 Быстрый старт

### Предварительные требования
- Visual Studio 2022 (с ASP.NET workload)
- .NET 8 SDK
- SQL Server 2022 (или LocalDB — встроен в VS)
- Node.js 18+ и npm

---

### 1️⃣ База данных

Откройте `appsettings.json` и проверьте строку подключения:

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GorodDeneg;Trusted_Connection=True;TrustServerCertificate=True;"
```

Для SQL Server 2022 измените на:
```json
"DefaultConnection": "Server=localhost;Database=GorodDeneg;Trusted_Connection=True;TrustServerCertificate=True;"
```

---

### 2️⃣ Запуск API (бэкенд)

```bash
cd GorodDeneg.API

# Создать первую миграцию (если нет папки Migrations)
dotnet ef migrations add InitialCreate

# Запустить — миграции и seed применятся автоматически при старте
dotnet run
```

API запустится на `https://localhost:7001`  
Swagger UI: `https://localhost:7001/swagger`

---

### 3️⃣ Запуск фронтенда

```bash
cd goroddeneg-frontend
npm install
npm run dev
```

Откройте `http://localhost:5173`

---

### 4️⃣ Демо-аккаунты

| Роль  | Email                    | Пароль         |
|-------|--------------------------|----------------|
| Пользователь | aibek@goroddeneg.kz | User@123456!  |
| Администратор | admin@goroddeneg.kz | Admin@123456! |

---

## 🔧 Конфигурация

### JWT Secret (обязательно поменять в продакшне!)
```json
"Jwt": {
  "Secret": "GorodDenegSuperSecretKey2024!MustBe32CharsMin_ChangeInProd",
  "Issuer": "GorodDeneg.API",
  "Audience": "GorodDeneg.Frontend",
  "ExpiryMinutes": 1440
}
```

### CORS
```json
"Cors": {
  "AllowedOrigins": [ "http://localhost:5173" ]
}
```

---

## 🗄️ База данных (таблицы)

| Таблица          | Описание                                      |
|------------------|-----------------------------------------------|
| `Users`          | ASP.NET Identity пользователи (AppUser)        |
| `Roles`          | Роли (Admin, User)                            |
| `Categories`     | Категории проектов (19 штук)                  |
| `Projects`       | Проекты с статусом, суммой, датами            |
| `Rewards`        | Вознаграждения для спонсоров                  |
| `Pledges`        | Записи о поддержке (платежи)                  |
| `Transactions`   | История операций с балансом                   |
| `LegalDetails`   | Юридические данные и банковские реквизиты     |
| `ProjectUpdates` | Обновления проекта                            |
| `ProjectMedia`   | Медиафайлы проекта (фото, видео)              |

---

## 📡 API Endpoints

### Auth
| Метод  | Путь               | Описание              |
|--------|--------------------|-----------------------|
| POST   | /api/auth/register | Регистрация           |
| POST   | /api/auth/login    | Вход                  |
| GET    | /api/auth/me       | Текущий пользователь  |
| POST   | /api/auth/change-password | Смена пароля   |

### Projects
| Метод  | Путь                           | Описание               |
|--------|--------------------------------|------------------------|
| GET    | /api/projects                  | Список проектов        |
| GET    | /api/projects/{id}             | Детали проекта         |
| GET    | /api/projects/my               | Мои проекты            |
| POST   | /api/projects                  | Создать проект         |
| POST   | /api/projects/{id}/cover       | Загрузить обложку      |
| POST   | /api/projects/{id}/package     | Выбрать пакет          |
| POST   | /api/projects/{id}/pledge      | Поддержать проект      |

### Users
| Метод  | Путь                    | Описание           |
|--------|-------------------------|--------------------|
| GET    | /api/users/profile      | Профиль            |
| PUT    | /api/users/profile      | Обновить профиль   |
| POST   | /api/users/avatar       | Загрузить аватар   |
| GET    | /api/users/transactions | История операций   |
| POST   | /api/users/topup        | Пополнить баланс   |
| GET    | /api/users/pledges      | Мои поддержки      |
| GET    | /api/users/legal        | Реквизиты          |
| POST   | /api/users/legal        | Сохранить реквизиты|

### Admin (`[Authorize(Roles = "Admin")]`)
| Метод  | Путь                              | Описание              |
|--------|-----------------------------------|-----------------------|
| GET    | /api/admin/stats                  | Статистика дашборда   |
| GET    | /api/admin/projects               | Все проекты           |
| PUT    | /api/admin/projects/{id}/status   | Изменить статус       |
| GET    | /api/admin/users                  | Все пользователи      |
| GET    | /api/admin/transactions           | Все транзакции        |

---

## 🎨 Дизайн-система

Шрифты: **Cormorant Garamond** (заголовки) + **DM Sans** (текст)  
Цветовая палитра: тёплая кремово-золотая тема

| Переменная       | Цвет        | Использование              |
|------------------|-------------|----------------------------|
| `--gold`         | `#A67C00`   | Акцент, кнопки, цены       |
| `--gold-subtle`  | `#FBF6E8`   | Фоны секций                |
| `--cream`        | `#FEFCF5`   | Основной фон               |
| `--text-primary` | `#2C1F0A`   | Основной текст             |
| `--text-muted`   | `#A08B60`   | Вспомогательный текст      |

---

## ⚙️ Команды EF Core (в папке GorodDeneg.API)

```bash
# Добавить миграцию
dotnet ef migrations add <Название>

# Применить миграции
dotnet ef database update

# Откатить последнюю
dotnet ef migrations remove

# Посмотреть все миграции
dotnet ef migrations list
```

---

## 📦 Статусы проектов

| Статус      | Описание                               |
|-------------|----------------------------------------|
| `Draft`     | Черновик (создан, не отправлен)        |
| `Pending`   | Ожидает модерации                      |
| `Active`    | Одобрен, идёт сбор средств            |
| `Rejected`  | Отклонён администратором              |
| `Completed` | Успешно завершён                       |
| `Failed`    | Не собрал нужную сумму                |

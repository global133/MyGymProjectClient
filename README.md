💪 MyGymProject — Онлайн-запись в тренажёрный зал

📝 Описание
MyGymProject — это веб-приложение для тренажёрного зала, позволяющее пользователям записываться на тренировки онлайн, просматривать расписание, выбирать тренеров, и управлять своим расписанием.
Приложение построено на ASP.NET Core Razor Pages (клиент) и Web API (сервер), с поддержкой кэширования данных через Redis.

🚀 Стек технологий

ASP.NET Core 8.0
Razor Pages для клиентского интерфейса
ASP.NET Core Web API для бизнес-логики
Entity Framework Core (база данных)
PostgreSQL
Redis (кэширование)
HttpClientFactory для безопасных и масштабируемых API-запросов
Bootstrap 5 (UI)
Toast уведомления, модальные окна и подтверждения

⚙️ Установка и запуск

1. Клонировать репозитории:
   git clone https://github.com/global133/MyGymProjectClient (клиентская часть)
   git clone https://github.com/global133/MyGymProjectServer (cерверная часть)

2. Запустить клиентскую часть:
   cd MyGymProjectClient
   dotnet run
   
3. Запустить серверную часть:
   cd MyGymProjectServer
   dotnet run


✅ Основной функционал
🔐 Аутентификация и авторизация
📅 Просмотр и фильтрация расписания
👤 Выбор и просмотр тренеров
💬 Уведомления (toast) при действиях пользователя
🧠 Кэширование данных с помощью Redis
⚡️ Быстрая загрузка за счёт кэширования и API оптимизации

 Примеры API-запросов
 GET /api/trainers/5
 POST /api/trainings/{sessionId}/clients/{clientId}

📌 Заметки
Адреса и порты API настроены в launchSettings.json и в HttpClient через IHttpClientFactory.
Redis используется как внешний кэш для снижения нагрузки на БД и ускорения отклика.
   

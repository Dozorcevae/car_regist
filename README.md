# 🚗 AutoService — веб-приложение для автосервиса

Привет! Это маленький (но гордый) проект, который помогает автосервису вести учёт машин, их владельцев и ремонтных работ, а клиентам — без нервов отслеживать, когда их железный конь снова выйдет на дорогу.

---

## Роли и права ✋

| Кто?                        | Что умеет?                                                                                                                                                                                                     |
| --------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Admin** (менеджер/мастер) | • Создавать/редактировать владельцев и их авто.<br>• Назначать дату/время ремонта.<br>• Добавлять набор работ из справочника.<br>• Закрывать ремонт, меняя статус на **Finished**.<br>• Видеть всю статистику. |
| **User / Client**           | • Залогиниться и увидеть ТОЛЬКО своё авто.<br>• Следить за статусом ремонта.<br>• Без доступа к чужим данным и ценам.                                                                                          |

> **По-умолчанию** при первом запуске сидятся две роли: `Admin`, `User`.

---

## Быстрый старт 🏁

# 1. Git clone
```bash
git clone https://github.com/Dozorcevae/car_registration_web.git
cd car_registration_web
```
### create appsettings.json
> Пример appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port={you_number_port};Database=car_registration;Username={user_name};Password={you_pass}"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/app-.log",
          "rollingInterval": "Day",
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
          "retainedFileCountLimit": 31
        }
      }
    ],
    "Enrich": [
      "FromLogContext"
    ]
  },
  "Identity": {
    "Password": {
      "RequireDigit": true,
      "RequireLowercase": true,
      "RequireUppercase": true,
      "RequireNonAlphanumeric": true,
      "RequiredLength": 6
    },
    "SignIn": {
      "RequireConfirmedAccount": false
    }
  },
  "AllowedHosts": "*",
  "BackupSettings": {
    "Path": "Backups",
    "RetentionDays": 30
  }
}
```
# 3. Pre-run command
```bash
npm install
npm run css:build
dotnet build
dotnet run
```

*Приложение само применит миграции и засеет справочник работ (6 штук) при первом старте.*
```c#
await context.Database.MigrateAsync();
```
> если по каким то `мистическим` причинам миграция не применяется ,то рекомендую использовать данную cmd
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```
> но как показали тесты , в 98% `await context.Database.MigrateAsync()` успешно справляется с поставленной задачей
---

## Create user

*(логины можно поменять в `DatabaseInitializer.EnsureSeedUsers()`)*

---

## Главное по технологиям ⚙️

* **ASP.NET Core 9** (MVC) + **EF Core** + **Identity**
* SQL Server (работает и на LocalDB, и Postgresql)
* Сид-данные через `DatabaseInitializer` (миграции + Seed).
* Frontend — Razor + чуточку Bootstrap, без SPA-перегруза.

---

## Как добавить новую работу ✍️

1. Открой `WorkTypes` в БД.<br>2. Вставь строку `('Замена свечей', 2200, 35)`.<br>3. Перезапусти сайт — чек-лист автоматически появится в форме администратора.
 ---

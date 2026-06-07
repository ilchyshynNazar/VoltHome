````md
# ☀️ VoltHome

VoltHome is a solar station monitoring and energy forecasting platform.
It combines a React + TypeScript frontend with an ASP.NET Core backend, PostgreSQL, and ML.NET.

---

## 🚀 What it does

- Tracks solar station production and equipment configuration
- Monitors energy generation and daily performance
- Forecasts solar output using ML.NET models
- Calculates green tariff revenue and payback period
- Supports secure JWT authentication and role-based access

---

## 🧩 Technology stack

- Frontend: React, TypeScript, Vite
- Backend: ASP.NET Core 8, Entity Framework Core, ASP.NET Identity
- Database: PostgreSQL
- Machine learning: ML.NET
- API docs: Swagger / OpenAPI

---

## 📁 Frontend structure

- `src/` — application components and pages
- `public/screenshots/` — example UI images
- `package.json` — frontend dependencies and scripts

---

## ⚙️ Run the frontend

```bash
cd frontend
npm install
npm run dev
```

---

## 🔧 Important links

- `backend/VoltHome.API` — API project for authentication and data services
- `backend/VoltHome.Services` — business logic and forecasting services

---

## 📸 Screenshots

Login screen:

![Login](../public/screenshots/login.png)

Solar station configuration:

![Configuration](../public/screenshots/configuration.png)

Monitoring dashboard:

![Dashboard](../public/screenshots/dashboard.png)

Green tariff calculations:

![Green tariff](../public/screenshots/green.png)

````md
# ☀️ VoltHome

AI-powered solar station management and energy forecasting platform built with ASP.NET Core, PostgreSQL and ML.NET.

---

## 🚀 Overview

VoltHome is a web application designed for monitoring solar power stations, forecasting energy generation, and calculating financial efficiency.

The platform allows users to configure solar stations, monitor production in real time, analyze profitability, and leverage machine learning models for generation prediction.

---

## ✨ Features

### 🔐 Authentication & Security

- ASP.NET Core Identity
- JWT Authentication
- Role-based authorization
- Secure password policies

### ☀️ Solar Station Management

- Create and manage solar stations
- Configure inverters
- Configure panel groups
- Multi-station support
- Flexible editing of station components

### 📊 Monitoring Dashboard

- Current generation monitoring
- Daily energy statistics
- Forecast visualization
- Automatic data refresh

### 🤖 Machine Learning Forecasting

- ML.NET integration
- Hour-ahead generation prediction
- Feature normalization
- Automated model execution

### 💰 Financial Analytics

- Installation cost estimation
- Green tariff calculations
- Revenue estimation
- Payback period analysis

### ⚙️ Background Processing

- Daily aggregations
- Scheduled calculations
- Forecast updates
- Background services

---

## 🏗️ Architecture

```text
Frontend
    │
    ▼
Controllers (API)
    │
    ▼
Services
    │
    ▼
Repositories
    │
    ▼
PostgreSQL

    ▲
    │
 ML.NET Forecast Engine
🤖 Machine Learning Module

The platform includes a machine learning forecasting engine responsible for predicting future solar energy generation.

Input Features
Feature	Description
Hour	Current hour
Month	Current month
PanelPowerKw	Installed panel power
InverterPowerKw	Inverter power
HourCoefficient	Hour generation coefficient
MonthCoefficient	Seasonal coefficient
AzimuthFactor	Panel orientation factor
TiltFactor	Tilt angle factor
ML Pipeline
Input Features
      │
      ▼
Feature Scaling
      │
      ▼
ML.NET Model
      │
      ▼
Generation Forecast

The architecture allows future migration toward more advanced forecasting models such as:

RNN
LSTM
Deep Learning Time-Series Models
🛠️ Technology Stack
Backend
ASP.NET Core 8
Entity Framework Core
ASP.NET Identity
JWT Authentication
FluentValidation
Database
PostgreSQL
Machine Learning
ML.NET
Documentation
Swagger / OpenAPI
Background Processing
Hosted Services
Background Workers
📁 Project Structure
VoltHome
│
├── VoltHome.API
├── VoltHome.Services
├── VoltHome.Infrastructure
├── VoltHome.Domain
├── VoltHome.Contracts
│
├── Authentication
├── Solar Stations
├── Analytics
├── Forecasting
├── Machine Learning
└── Background Services
⚙️ Getting Started
Prerequisites
.NET 8 SDK
PostgreSQL
Visual Studio 2022 / Rider
Clone Repository
git clone https://github.com/your-username/VoltHome.git
cd VoltHome
Configure Database

Update appsettings.json:

"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5440;Database=volt-home;Username=postgres;Password=1111"
}
Apply Migrations
dotnet ef database update
Run Application
dotnet run
🔑 Default Accounts
Administrator
Email: internalAdmin@gmail.com
Password: Qwerty123$
Test User
Email: john@example.com
Password: Test123$
📖 API Documentation

Swagger UI:

https://localhost:<port>/swagger
📸 Screenshots
Login
![Login](frontend/public/screenshots/login.png)
Solar Station Configuration
![Login](frontend/public/configuration/login.png)
Monitoring Dashboard
![Dashboard](frontend/public/screenshots/dashboard.png)
Green tariff calculations
![Dashboard](frontend/public/screenshots/green.png)

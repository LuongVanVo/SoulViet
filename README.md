# 🌟 SoulViet Backend API

Welcome to **SoulViet Backend API**! This project serves as the core backend infrastructure for the SoulViet ecosystem. Built on **.NET 10**, it utilizes a highly scalable **Modular Architecture** to keep features decoupled, maintainable, and easy to extend. 

The system leverages modern technologies for relational data, in-memory caching, and asynchronous messaging to ensure high performance and reliability.

## 🛠 Technology Stack
- **Framework**: .NET 10 (C#)
- **Database**: PostgreSQL (Relational Database)
- **Cache Strategy**: Redis (Distributed Caching)
- **Message Broker**: RabbitMQ (Asynchronous Event Processing)
- **Containerization**: Docker & Docker Compose
- **API Documentation**: Swagger
- **Monitoring**: Built-in Health Checks

---

## 📂 Project Architecture (Modular Monolith)

The backend is structured using a Modular Monolith approach. This means the application runs as a single process but is logically divided into self-contained modules based on business domains.

### Core Structure:
- **`SoulViet.API`**: The main host, API Gateway, and entry point of the application. It handles routing, middleware, and dependency injection composition.
- **`SoulViet.Shared.Infrastructure`**: Contains common utilities, cross-cutting configurations (database contexts, messaging templates), and shared components used across the entire system.

### Business Modules:
- **📦 `Marketplace Module` (`Modules/SoulViet.Modules.Marketplace`)**: Manages the e-commerce aspects, including product listings, orders, and transactions.
- **🌐 `Social Module` (`Modules/SoulViet.Modules.Social`)**: Handles social networking features, user interactions, feeds, and community engagement.
- **🗺️ `SoulMap Module` (`Modules/SoulViet.Modules.SoulMap`)**: Manages geospatial data, mapping functionalities, and location-based services.

---

## 🚀 Quick Overview for Development

- **Environment setup:** The project relies on a `.env` file for all configuration secrets and environment variables.
- **Infrastructure:** All backing services (PostgreSQL, Redis, RabbitMQ) are quickly orchestrated using `docker-compose.yml` located at the root.
- **Local Startup:** Once the infrastructure is running via Docker, the `SoulViet.API` project is set as the startup project and handles the bootstrapping of all individual modules.

> *For detailed setup instructions, environment configurations, and deployment guidelines, please refer to the internal development documentation or Wiki.*
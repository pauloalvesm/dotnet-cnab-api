<h1 align="center">💵 CNAB - API</h1>

<p align="center">
  <a href="https://learn.microsoft.com/pt-br/dotnet/"><img alt="DotNet 6" src="https://img.shields.io/badge/.NET-5C2D91?logo=.net&logoColor=white&style=for-the-badge" /></a>
  <a href="https://learn.microsoft.com/pt-br/dotnet/csharp/programming-guide/"><img alt="C#" src="https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white&style=for-the-badge" /></a>
  <a href="https://www.postgresql.org/"><img alt="PostgreSQL" src="https://img.shields.io/badge/postgres-%23316192.svg?style=for-the-badge&logo=postgresql&logoColor=white" /></a>
  <a href="https://www.docker.com/"><img alt="Docker" src="https://img.shields.io/badge/docker-%230db7ed.svg?style=for-the-badge&logo=docker&logoColor=white" /></a>
  <a href="LICENSE"><img alt="License: MIT" src="https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge" /></a>
</p>

<p align="center"> <img src="https://github.com/pauloalvesm/dotnet-cnab-api/blob/master/src/CNAB.WebAPI/Resources/Images/screenshot1.png?raw=true" /></p>

## 💻 Project

This repository contains a Web API designed to manage `CNAB` data by handling records and supporting file uploads for data normalization.

The project is an `MVP (Minimum Viable Product)` developed for academic purposes, inspired by the following challenge: [CNAB Challenge](https://github.com/PauloAlves8039/dotnet-desafio-cnab).

File used for testing: [CNAB.txt](https://github.com/pauloalvesm/dotnet-cnab-api/blob/master/src/CNAB.WebAPI/Resources/Files/CNAB.txt).

## 📘 Business rule

This project primarily focuses on the import and normalization of financial transactions. Data entry can happen in two ways: either by uploading `CNAB` files or through direct input via fields tied to the `Store` and `Transaction` entities. The web application handles processing the CNAB data specifically, it divides transaction values by 100 for proper normalization and then stores everything in a relational database. Afterward, the system displays a list of operations grouped by store, complete with a running total balance for each.

To manage this information, the system includes full `CRUD (Create, Read, Update, Delete)` capabilities for the Store entity, allowing users to list, view details of, create, update, and delete stores. Similarly, the Transaction entity also boasts comprehensive CRUD support, enabling individual management of its operations.

It's worth noting that the CNAB file parsing service is accessible via the `/upload-cnab-file` endpoint within the CNABController, serving as the entry point for data processing and persistence. On top of that, we've implemented a dedicated area that provides exclusive metrics and insights for administrator users.

## 📌 Technical Decisions

- `Web API`:  I chose to build an API to provide more flexibility for developing a front-end application using a modern SPA framework.
- `Clean Architecture`: The goal of applying this architecture was to make the API scalable and well-structured for adding new features.
- `Rich Domain Model`: This approach was used to organize domain classes and align them closely with the application's business rules.

## 🚀 Technologies and Tools

This project was built using the following stack:

- **Backend:**  
  - `.NET 8.0`
  - `ASP.NET Core WebAPI`
  - `C#`
  - `Entity Framework Core`
  - `ASP.NET Core Identity`
  - `JWT`
  - `Mapster`
  - `Clean Architecture`
  - `Rich Domain Model`

- **Testing:**  
  - `XUnit`
  - `Moq`
  - `FluentAssertions`

- **Infrastructure:**  
  - `PostgreSQL`
  - `Docker`
  - `Inversion of Control`

## 💾 How to Run Locally

```bash
# Clone the repository
git clone https://github.com/pauloalvesm/dotnet-cnab-api.git

# Navigate to the project folder
cd src/CNAB.WebAPI

# Restore dependencies
dotnet restore

# Run the project
dotnet run
```

## 🧪 How to Run (Docker)

```bash
# Clone the repository
git clone https://github.com/pauloalvesm/dotnet-cnab-api.git

# Navigate to the project folder
cd src/CNAB.WebAPI

# Run the command to build the image and start the container
docker-compose up --build
```

## ⬇️ How to Use

```bash
# After creating and starting the containers, restore the database tables using the following commands:

# Run migrations created with ApplicationDbContext
dotnet ef database update --context CNAB.Infra.Data.Context.ApplicationDbContext --startup-project ../CNAB.WebAPI

# Run migrations created with IdentityApplicationDbContext
dotnet ef database update --context CNAB.Infra.Data.Context.IdentityApplicationDbContext --startup-project ../CNAB.WebAPI

# Commands used to create the migrations:

# For ApplicationDbContext
dotnet ef migrations add InitialCreate --context CNAB.Infra.Data.Context.ApplicationDbContext --startup-project ../CNAB.WebAPI

# For IdentityApplicationDbContext
dotnet ef migrations add AddIdentity --context CNAB.Infra.Data.Context.IdentityApplicationDbContext --startup-project ../CNAB.WebAPI

```

## ℹ️ Another option for restoring tables in the database

Run these scripts directly on the database created using Docker, so that the tables will be created based on their contexts:

[ApplicationDbContext.sql](https://github.com/pauloalvesm/dotnet-cnab-api/blob/master/src/CNAB.WebAPI/Resources/Scripts/ApplicationDbContext.sql)

[IdentityApplicationDbContext.sql](https://github.com/pauloalvesm/dotnet-cnab-api/blob/master/src/CNAB.WebAPI/Resources/Scripts/IdentityApplicationDbContext.sql)


## 🔒 Authentication and Authorization
```bash
# After setting up the API to access the endpoints, you must create a user with the following email:

admin@localhost

# You can set any password you prefer.

Example: YourPassword@2015

# Note: Once created, the admin@localhost user will have access to all API endpoints, including an admin-only section.
# All other users will have access to the API, except for the admin-only section.

```

## 🌎 URL

```bash
# Whether you're running the project locally or inside a Docker container, the URL remains the same.
# The API will be available at:

http://localhost:8080/swagger/index.html

```

## 🎯 API Endpoints

- **Base URL(Store, Transaction, User):** `http://localhost:8080`
- **Admin URL:** `http://localhost:8080/api/Admin`
- **CNAB URL:** `http://localhost:8080/CNAB`

### 🛠️ Admin

| Method      | Endpoint                                                 | Description                                  |
|-------------|---------------------------------------|-----------------------------------------------------------------|
| `GET`       | `/total-balance`     | Retrieves the total balance of all transactions.  |
| `GET`       | `/store-count`       | Returns the total count of registered stores.     |
| `GET`       | `/transaction-count` | Returns the total count of transactions.          |

---

### 📄 CNAB

| Method      | Endpoint                                                 | Description                                  |
|-------------|------------------------------------|--------------------------------------------------------------------|
| `POST`      | `/upload-cnab-file` | Uploads a CNAB file for processing.                     |

---

### 🏬 Store

| Method      | Endpoint                                                 | Description                                  |
|-------------|------------------------------------|--------------------------------------------------------------------|
| `GET`       | `/api/Store`      | Lists all stores.                                              |
| `GET`       | `/api/Store/{id}` | Gets details of a specific store by ID.                        |
| `POST`      | `/api/Store`      | Creates a new store.                                           |
| `PUT`       | `/api/Store/{id}` | Updates an existing store by ID.                               |
| `DELETE`    | `/api/Store/{id}` | Deletes an existing store by ID.                               |

---

### 💳 Transaction

| Method      | Endpoint                                                 | Description                                   |
|-------------|---------------------------------------------|------------------------------------------------------------|
| `GET`       | `/api/Transaction`        | Lists all transactions.                                 |
| `GET`       | `/api/Transaction/{id}`        | Gets details of a specific transaction by ID.      |
| `POST`      | `/api/Transaction`        | Creates a new transaction.                              |
| `PUT`       | `/api/Transaction/{id}`   | Updates an existing transaction by ID.                  |
| `DELETE`    | `/api/Transaction/{id}`   | Deletes an existing transaction by ID.                  |

---

### 👥 User

| Method      | Endpoint                                                 | Description                                   |
|-------------|------------------------------------|---------------------------------------------------------------------|
| `POST`      | `/api/User/register` | Registers a new user.                                        |
| `POST`      | `/api/User/login`    | Logs in a user.                                              |


## 👤 Author

**[Paulo Alves](https://github.com/pauloalvesm)**

## 📝 License

This project is licensed under the [MIT License](LICENSE).
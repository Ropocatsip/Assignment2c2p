# 2C2P Technical Assignment

This repository contains the implementation for **Assignment 1** (Payment Gateway API) and **Assignment 2** (Financial Reconciliation Tool).

---

## 📌 Prerequisites
- **.NET 10 SDK**
- **MongoDB** (Local or MongoDB Atlas instance)

---

## 🚀 Assignment 1: Payment Gateway API (`Assignment1.API`)

A RESTful Web API built with **.NET 10** providing payment processing services with request encryption, validation, and database persistence.

### 🛠️ Configuration & Database Setup
Before running the API, update your database configuration in `appsettings.Development.json`:
1. Open `Assignment1.API/appsettings.Development.json`.
2. Set the `MONGODBURI` property with your personal database connection string from **`Assignment Answer.pdf`**.
   > ℹ️ *If you did not receive the `Assignment Answer.pdf` file, please contact **nudthaya.opor@gmail.com**.*

### ⚡ Running the API
To launch the Payment API:
```bash
dotnet run --project Assignment1.API
```
The server will start listening at `http://localhost:5279`.

### 📚 API Documentation & Swagger UI
Interactive Swagger API documentation is available at:
👉 **[http://localhost:5279/swagger/index.html](http://localhost:5279/swagger/index.html)**

### 🔐 Client Identity & Authentication
- **Authentication Header**: `X-Api-Key`
- **Default API Key**: `assignment1-1234`

### 🛡️ Duplicate Payment Handling (Idempotency)
To prevent accidental double-charging if a user clicks the **"Pay"** button twice:
- The system queries the database before processing to verify whether the `OrderNumber` already has a transaction with status `'APPROVED'`.
- If an approved transaction exists for that order, the API immediately rejects the request with a `400 Bad Request` error (`Order number '...' has already been paid and approved.`).

### 🧪 Unit Testing (`Assignment1.TEST`)
Run the test suite for Assignment 1:
```bash
dotnet test Assignment1.TEST
```

### 📬 Postman Collection
An attached Postman collection is provided for endpoint testing.
> ℹ️ *If you did not receive the Postman collection file, please contact **nudthaya.opor@gmail.com**.*

---

## 📊 Assignment 2: Financial Reconciliation Tool (`Assignment2.APP`)

A high-performance console application built with **.NET 10** for processing and reconciling financial records between Order transactions (**List A**) and Invoice transactions (**List B**).

### ⚡ Running the Application
To run the reconciliation tool:
```bash
dotnet run --project Assignment2.APP
```
* **Input CSV Files**: Located in `Assignment2.APP/Data/` (`List A - List1.csv` and `List B - List2.csv`).
* **Output CSV Files**: Generated in `Assignment2.APP/output/` (`Matched_Records.csv`, `Missing_In_A.csv`, `Missing_In_B.csv`).

### 🧪 Unit Testing (`Assignment2.TEST`)
Run the test suite for Assignment 2:
```bash
dotnet test Assignment2.TEST
```

---
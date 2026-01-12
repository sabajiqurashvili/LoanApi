# Loan API

RESTful Web API სესხების მართვისთვის, რომელიც უზრუნველყოფს მომხმარებლისა და ბუღალტრის (**Accountant**) როლებზე დაფუძნებულ ავტორიზაციას, სესხის მოთხოვნას, მართვას და კონტროლს.

---

## 📌 Overview

**Loan API** საშუალებას აძლევს მომხმარებლებს მოითხოვონ სესხი, ნახონ და მართონ მხოლოდ საკუთარი სესხები, ხოლო **Accountant** როლს აქვს სრული წვდომა ყველა სესხზე და მომხმარებლების დაბლოკვის შესაძლებლობა.

პროექტი აგებულია **REST principles**-ის მიხედვით და იყენებს:
- JWT Authentication
- Role-Based Authorization
- Clean Architecture მიდგომას

---

## 🧰 Tech Stack

- **Language:** C#
- **Framework:** ASP.NET Core Web API
- **Database:** SQL Server
- **ORM:** Entity Framework Core
- **Authentication:** JWT (Json Web Token)
- **Authorization:** Role Based (User, Accountant)
- **Validation:** FluentValidation
- **Logging:** Serilog (File logging)
- **Testing:** xUnit, Moq
- **Documentation:** Swagger (OpenAPI)

---

## 👥 Roles

### 👤 User
- რეგისტრაცია და ავტორიზაცია
- საკუთარი პროფილის ნახვა
- სესხის მოთხოვნა
- მხოლოდ საკუთარი სესხების ნახვა / განახლება / წაშლა
- ვერ ცვლის სესხის სტატუსს
- ვერ ითხოვს სესხს თუ `IsBlocked = true`

### 👨‍💼 Accountant
- ყველა მომხმარებლის სესხის ნახვა
- სესხის სტატუსის შეცვლა
- ნებისმიერი სესხის წაშლა
- მომხმარებლის დაბლოკვა გარკვეული დროით

---

## 💳 Loan Entity

### Loan Fields

| Field | Description |
|------|------------|
| LoanType | Fast, Auto, Installment |
| Amount | Loan amount |
| Currency |GEL USD EUR| 
| Period | Loan duration (months) |
| Status | Processing, Approved, Rejected |

📌 სესხის შექმნისას სტატუსი ავტომატურად არის **Processing**.

---

## 🚀 API Endpoints

### 🔐 Authentication

| Method | Endpoint | Description |
|------|--------|-------------|
| POST | `/api/auth/register` | User registration |
| POST | `/api/auth/login` | User login (JWT) |

---

### 👤 User – Loans

| Method | Endpoint | Description |
|------|--------|-------------|
| POST | `/api/loans` | Request a new loan |
| GET | `/api/loans/my` | Get my loans |
| GET | `/api/loans/{id}` | Get my loan by id |
| PUT | `/api/loans/{id}` | Update loan (only Processing) |
| DELETE | `/api/loans/{id}` | Delete loan (only Processing) |

---

### 👨‍💼 Accountant – Loans

| Method | Endpoint | Description |
|------|--------|-------------|
| GET | `/api/accountant/loans` | Get all loans |
| DELETE | `/api/accountant/loans/{id}` | Delete any loan |

---

### 👨‍💼 Accountant – Users

| Method | Endpoint | Description |
|------|--------|-------------|
| PUT | `/api/users/block/{id}` | Block user |

---

## ⚙️ Configuration

**appsettings.json**
```json
{
  "ConnectionStrings": {
    "BankAppEntityFrameworkWEBAPI": "Server=DESKTOP-SPEG7LL\\SQLEXPRESS;Database=BankApp;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "super-secret-key",
    "Issuer": "LoanAPI",
    "Audience": "LoanAPIUsers"
  }
}

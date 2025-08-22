# 📚 Library Management API

A RESTful Web API built with ASP.NET Core using **Entity Framework Core scaffolding** to generate models from an existing SQL Server database. Currently working on improvements to JWT authentication to better increase security and align more with industry standards. To do: add angular front end. 

## 🚀 Technologies Used

- ASP.NET Core 9.0
- Entity Framework Core (Database-First via Scaffolding)
- SQL Server
- AutoMapper
- JWT Authentication
- Swagger (OpenAPI)

## 📦 Features

- ✅ CRUD operations for Books and Students
- 📖 Book checkout and return tracking
- 🔐 Role-based authentication (`Admin`, `User`)
- 📄 JSON Patch support for partial updates
- 📊 Swagger UI for interactive API documentation

## 🛠️ Getting Started

### Prerequisites

- [.NET SDK 9.0](https://dotnet.microsoft.com/download)
- SQL Server (local or cloud)
- Visual Studio or VS Code

### Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/library-mgmt-api.git
   cd library-mgmt-api
2. Creating database schema:
   Run .sql commands in the SQL folder in Microsoft SQL Server Manangement Studio.
3. Scaffold your models and DbContext from the database:
   dotnet ef dbcontext scaffold "Server=localhost;Database=library_mgmt;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models --context DataContext --force
   dotnet tool install --global dotnet-ef
4. Update the connection string in appsettings.json:
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=library_mgmt;Trusted_Connection=True;"
   }
5. Run the application:
   dotnet run
6. Open Swagger UI:
   https://localhost:5001/swagger

🔐 Authentication
This API uses JWT Bearer tokens. To access protected endpoints:

Authenticate via /api/auth/login

Include the token in the Authorization header:

Authorization: Bearer <your-token>

📚 API Endpoints
Method	Endpoint	Description
GET	/api/books	Get all books
GET	/api/books/{id}	Get book by ID
POST	/api/books	Add a new book
PATCH	/api/books/{id}	Update book (partial)
GET	/api/students	Get all students
POST	/api/students	Add a new student
POST	/api/books/checkout	Checkout a book
POST	/api/books/return	Return a book

🧪 Testing
Unit and integration tests are located in the LibraryMgmt.Tests project. Run tests with:
dotnet test

📄 License
This project is licensed under the MIT License. See the LICENSE file for details.

   

# 📚 BookLibrary API

A RESTful API built with **ASP.NET Core 8** demonstrating **Clean Architecture** principles, Entity Framework Core, and modern backend development best practices.

## 🎯 Project Overview

This project serves as a demonstration of my expertise in:
- **Clean Architecture** implementation with clear separation of concerns
- **RESTful API** design following industry standards
- **Entity Framework Core** with MySQL database
- **Repository Pattern** and **Dependency Injection**
- **AutoMapper** for object-to-object mapping
- **Global Exception Handling** middleware
- **SOLID principles** and maintainable code structure

## 🏗️ Architecture

The solution follows **Clean Architecture** principles with three distinct layers:

```
BookLibrary/
├── BookLibrary.Api/          # Presentation Layer
│   ├── Controllers/              # API endpoints
│   ├── Models/        # DTOs (Data Transfer Objects)
│   ├── Profiles/ # AutoMapper profiles
│   └── Middleware/   # Custom middleware (Exception handling)
│
├── BookLibrary.Core/             # Domain Layer
│   ├── Entities/       # Domain entities (TBook, TAuthor)
│   ├── Interfaces/          # Service and Repository interfaces
│   ├── Models/ # Domain models (BookItem, AuthorItem)
│   ├── Profiles/      # Domain mapping profiles
│   └── Exceptions/# Custom exceptions
│
└── BookLibrary.Infrastructure/   # Infrastructure Layer
    ├── Data/         # DbContext and Migrations
    ├── Repository/           # Repository implementations
    └── Services/     # Business logic services
```

### Layer Responsibilities

#### 🎨 Presentation Layer (BookLibrary.Api)
- Exposes HTTP endpoints via Controllers
- Handles request/response DTOs
- Implements global exception handling middleware
- Configures dependency injection and Swagger documentation

#### 💼 Domain Layer (BookLibrary.Core)
- Contains business entities and domain models
- Defines service and repository interfaces
- Enforces business rules and validations
- **Zero dependencies** on external frameworks

#### 🔧 Infrastructure Layer (BookLibrary.Infrastructure)
- Implements data access with Entity Framework Core
- Provides repository pattern implementations
- Handles database migrations
- Implements business logic services

## 🚀 Technologies & Patterns

### Technologies
- **.NET 8** - Latest LTS version
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core 9** - ORM for data access
- **MySQL** - Relational database (via Pomelo.EntityFrameworkCore.MySql)
- **AutoMapper 15** - Object-to-object mapping
- **Swagger/OpenAPI** - API documentation

### Design Patterns & Principles
- ✅ **Clean Architecture** - Separation of concerns across layers
- ✅ **Repository Pattern** - Abstraction over data access
- ✅ **Dependency Injection** - Loose coupling and testability
- ✅ **DTOs** - Separation between domain models and API contracts
- ✅ **SOLID Principles** - Single responsibility, dependency inversion
- ✅ **Async/Await** - Non-blocking I/O operations
- ✅ **CancellationTokens** - Graceful request cancellation

## 📋 Features

### API Endpoints

#### 📖 Books
- `GET /api/books` - Get all books (includes author information)
- `GET /api/books/{id}` - Get book by ID
- `POST /api/books` - Create a new book
- `PUT /api/books/{id}` - Update an existing book
- `DELETE /api/books/{id}` - Delete a book

#### ✍️ Authors
- `GET /api/author` - Get all authors (includes list of book titles)
- `GET /api/author/{id}` - Get author by ID
- `POST /api/author` - Create a new author
- `PUT /api/author/{id}` - Update an existing author
- `DELETE /api/author/{id}` - Delete an author

### Key Features
- **Eager Loading** - Navigation properties loaded with `.Include()`
- **Data Validation** - Business rule enforcement in service layer
- **Global Exception Handling** - Consistent error responses via middleware
- **AutoMapper Integration** - Seamless mapping between layers
- **CORS Support** - Cross-origin resource sharing enabled
- **Swagger UI** - Interactive API documentation

## 🗄️ Database Schema

```sql
┌─────────────────┐         ┌─────────────────┐
│   Author   │    │      Books      │
├─────────────────┤         ├─────────────────┤
│ Id (PK)         │◄───────┤│ Id (PK)         │
│ Name         │1:N ││ Title      │
│ BirthDate│         ││ PublishedDate   │
└─────────────────┘         ││ AuthorId (FK)   │
        └─────────────────┘
```

**Relationship**: One Author can have many Books (One-to-Many)

## ⚙️ Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/)
- Visual Studio 2022 or VS Code (optional)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/iachb/BookLibrary.git
   cd BookLibrary
   ```

2. **Configure Database Connection**
   
   Update `appsettings.json` in `BookLibrary.Api`:
 ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=BookLibraryDb;User=root;Password=yourpassword;"
     }
   }
   ```

3. **Apply Migrations**
   ```bash
   cd BookLibrary.Api
 dotnet ef database update
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```

5. **Access Swagger UI**

   Navigate to: `https://localhost:7xxx/swagger` (port may vary)

## 📦 NuGet Packages

### BookLibrary.Api
```xml
<PackageReference Include="AutoMapper" Version="15.1.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.10" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.10" />
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="9.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="9.0.6" />
```

### BookLibrary.Core
```xml
<PackageReference Include="AutoMapper" Version="15.1.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.10" />
```

### BookLibrary.Infrastructure
```xml
<PackageReference Include="AutoMapper" Version="15.1.0" />
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="9.0.0" />
```

## 🎓 Skills Demonstrated

This project showcases my proficiency in:

- ✅ Building scalable, maintainable backend systems
- ✅ Implementing Clean Architecture in .NET
- ✅ Working with Entity Framework Core and relational databases
- ✅ Creating RESTful APIs following best practices
- ✅ Applying SOLID principles and design patterns
- ✅ Writing async/await code for optimal performance
- ✅ Implementing proper error handling and validation
- ✅ Using dependency injection for loose coupling
- ✅ Documenting APIs with Swagger/OpenAPI

## 🤝 Contributing

This is a portfolio project, but feedback and suggestions are welcome! Feel free to open an issue or submit a pull request.

## 📄 License

This project is open source and available under the [MIT License](LICENSE).

## 👤 Author

- GitHub: [@iachb](https://github.com/iachb)

---

⭐ **If this project demonstrates valuable skills, please consider starring it!**
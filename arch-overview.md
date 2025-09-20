# ShaiebLibrary System Design & Architecture

## 1. System Overview

**Project Name**: ShaiebLibrary
**Description**: A comprehensive microservices-based library management system for managing a personal library of 1000+ books in English, Russian, and Arabic languages.

**Core Features**:
- Book catalog management with multilingual support
- User registration and management
- Book lending and return system
- Reservation system
- Notification system
- Analytics and reporting

## 2. Architecture Design

### 2.1 Overall Architecture
```
Frontend (Blazor Web App)
    ↓
API Gateway (Custom Ocelot)
    ↓
┌─────────────────────────────────────────┐
│           Microservices                 │
├─────────────┬─────────────┬─────────────┤
│Book Catalog │User Mgmt    │Lending      │
│Service      │Service      │Service      │
├─────────────┼─────────────┼─────────────┤
│Notification │Analytics    │Payment      │
│Service      │Service      │Service      │
└─────────────┴─────────────┴─────────────┘
    ↓
┌─────────────────────────────────────────┐
│         Infrastructure                  │
├─────────────┬─────────────┬─────────────┤
│SQL Server   │Azure Service│Azure Blob   │
│Databases    │Bus          │Storage      │
└─────────────┴─────────────┴─────────────┘
```

### 2.2 Microservices Architecture Pattern
- **Pattern**: Clean Architecture with Domain-Driven Design
- **Communication**: HTTP REST APIs + Event-driven messaging
- **Data**: Database per service
- **Deployment**: Independent containerized services

## 3. Microservices Breakdown

### 3.1 Book Catalog Service
**Purpose**: Manage books, authors, publishers, categories
**Technology**: .NET 9.0, Entity Framework Core, SQL Server
**Database**: BookCatalogDb

**Responsibilities**:
- Book CRUD operations
- Author and publisher management
- Category/genre management
- Book search and filtering
- Multilingual book metadata
- Book availability status management

**Entities**:
- Book, Author, Publisher, Category
- BookAuthor (M:M), BookCategory (M:M)

**API Endpoints**:
```
GET    /api/books
POST   /api/books
GET    /api/books/{id}
PUT    /api/books/{id}
DELETE /api/books/{id}
GET    /api/books/search?query=...&language=...
GET    /api/authors
POST   /api/authors
GET    /api/categories
POST   /api/categories
PUT    /api/books/{id}/availability
```

### 3.2 User Management Service
**Purpose**: Handle user accounts, authentication, profiles
**Technology**: .NET 9.0, Entity Framework Core, SQL Server
**Database**: UserManagementDb

**Responsibilities**:
- User registration and authentication
- User profile management
- Role-based access control
- Password management
- User preferences

**Entities**:
- User, UserProfile, UserRole, UserPreference

**API Endpoints**:
```
POST   /api/auth/register
POST   /api/auth/login
POST   /api/auth/refresh
GET    /api/users/profile
PUT    /api/users/profile
GET    /api/users/{id}
PUT    /api/users/{id}/role
```

### 3.3 Lending Service
**Purpose**: Manage book borrowing, returns, reservations
**Technology**: .NET 9.0, Entity Framework Core, SQL Server
**Database**: LendingDb

**Responsibilities**:
- Book checkout/checkin operations
- Loan period management
- Reservation system
- Due date tracking
- Loan history

**Entities**:
- Loan, Reservation, LoanHistory

**API Endpoints**:
```
POST   /api/loans/checkout
POST   /api/loans/return
GET    /api/loans/user/{userId}
GET    /api/loans/book/{bookId}
POST   /api/reservations
GET    /api/reservations/user/{userId}
DELETE /api/reservations/{id}
```

### 3.4 Notification Service
**Purpose**: Send notifications for due dates, overdue books
**Technology**: .NET 9.0, Azure Service Bus, SendGrid
**Database**: NotificationDb

**Responsibilities**:
- Email notifications
- SMS notifications (future)
- Push notifications (future)
- Notification templates
- Delivery tracking

### 3.5 Analytics Service
**Purpose**: Generate reports and analytics
**Technology**: .NET 9.0, Entity Framework Core, SQL Server
**Database**: AnalyticsDb

**Responsibilities**:
- Usage statistics
- Popular books reports
- User activity tracking
- Financial reports
- Dashboard data

### 3.6 Payment Service (Optional)
**Purpose**: Handle late fees, membership fees
**Technology**: .NET 9.0, Stripe/PayPal integration
**Database**: PaymentDb

## 4. Database Design

### 4.1 Database Strategy
- **Pattern**: Database per microservice
- **Type**: SQL Server for all services
- **Deployment**: Azure SQL Database

### 4.2 Book Catalog Database Schema
```sql
-- Books table
CREATE TABLE Books (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(500) NOT NULL,
    Subtitle NVARCHAR(500),
    ISBN VARCHAR(13) UNIQUE,
    ISBN13 VARCHAR(17),
    Language INT NOT NULL, -- Enum: 1=English, 2=Russian, 3=Arabic
    PublishedDate DATETIME2,
    Pages INT,
    Description NTEXT,
    Status INT NOT NULL, -- Enum: 1=Available, 2=CheckedOut, etc.
    Quantity INT NOT NULL DEFAULT 1,
    AvailableQuantity INT NOT NULL DEFAULT 1,
    Price DECIMAL(10,2),
    CoverImageUrl NVARCHAR(500),
    PublisherId INT,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
);

-- Authors table
CREATE TABLE Authors (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Biography NTEXT,
    BirthDate DATETIME2,
    DeathDate DATETIME2,
    PhotoUrl NVARCHAR(500),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
);

-- Publishers table
CREATE TABLE Publishers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Address NVARCHAR(500),
    Website NVARCHAR(200),
    EstablishedDate DATETIME2,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
);

-- Categories table
CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    ParentCategoryId INT,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (ParentCategoryId) REFERENCES Categories(Id)
);

-- BookAuthors (Many-to-Many)
CREATE TABLE BookAuthors (
    BookId INT,
    AuthorId INT,
    Role INT DEFAULT 1, -- Enum: 1=Author, 2=CoAuthor, 3=Editor, 4=Translator
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    PRIMARY KEY (BookId, AuthorId),
    FOREIGN KEY (BookId) REFERENCES Books(Id),
    FOREIGN KEY (AuthorId) REFERENCES Authors(Id)
);

-- BookCategories (Many-to-Many)
CREATE TABLE BookCategories (
    BookId INT,
    CategoryId INT,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    PRIMARY KEY (BookId, CategoryId),
    FOREIGN KEY (BookId) REFERENCES Books(Id),
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);
```

## 5. Technology Stack

### 5.1 Backend Services
- **Framework**: .NET 9.0
- **Database**: SQL Server / Azure SQL Database
- **ORM**: Entity Framework Core 9.0
- **Architecture**: Clean Architecture
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **API Documentation**: Swagger/OpenAPI

### 5.2 Infrastructure
- **Cloud Provider**: Microsoft Azure
- **Containers**: Docker
- **API Gateway**: Ocelot (Custom)
- **Message Bus**: Azure Service Bus
- **File Storage**: Azure Blob Storage
- **Monitoring**: Application Insights
- **Logging**: Serilog
- **Caching**: Azure Redis Cache (future)

### 5.3 Frontend
- **Framework**: Blazor Server/WebAssembly
- **Alternative**: React.js with TypeScript
- **UI Library**: Bootstrap / Material UI
- **State Management**: Built-in Blazor state

### 5.4 DevOps
- **Source Control**: Git (GitHub)
- **CI/CD**: GitHub Actions
- **Container Registry**: Azure Container Registry
- **Deployment**: Azure App Service / Azure Container Instances
- **Infrastructure as Code**: Azure Bicep

## 6. Functional Requirements

### 6.1 Book Management
- **BM-001**: Add new books with metadata (title, author, ISBN, language, etc.)
- **BM-002**: Edit book information
- **BM-003**: Delete books (soft delete)
- **BM-004**: Search books by title, author, ISBN, language
- **BM-005**: Filter books by category, language, availability
- **BM-006**: Manage book categories and subcategories
- **BM-007**: Handle multiple authors per book
- **BM-008**: Support multiple copies of the same book
- **BM-009**: Upload and manage book cover images

### 6.2 User Management
- **UM-001**: User registration with email verification
- **UM-002**: User login/logout
- **UM-003**: Password reset functionality
- **UM-004**: User profile management
- **UM-005**: Role-based access (Admin, Librarian, Member)
- **UM-006**: User preferences (language, notifications)

### 6.3 Lending System
- **LS-001**: Check out books to users
- **LS-002**: Return books
- **LS-003**: Reserve books when unavailable
- **LS-004**: Set loan periods (default 14 days)
- **LS-005**: Extend loan periods
- **LS-006**: Track overdue books
- **LS-007**: Calculate late fees
- **LS-008**: Loan history tracking
- **LS-009**: Prevent borrowing if user has overdue books

### 6.4 Notification System
- **NS-001**: Email reminders for due dates (3 days before)
- **NS-002**: Overdue notifications
- **NS-003**: Book availability notifications for reservations
- **NS-004**: Welcome emails for new users
- **NS-005**: Notification preferences management

### 6.5 Analytics & Reporting
- **AR-001**: Most popular books report
- **AR-002**: User activity reports
- **AR-003**: Overdue books report
- **AR-004**: Collection statistics (by language, category)
- **AR-005**: Financial reports (fees collected)
- **AR-006**: Usage dashboard for admin

## 7. Non-Functional Requirements

### 7.1 Performance
- **API response time**: < 500ms for 95% of requests
- **Database queries**: < 200ms for single record retrieval
- **Search performance**: < 1 second for book searches
- **Concurrent users**: Support 50+ concurrent users

### 7.2 Scalability
- **Horizontal scaling**: Each microservice can scale independently
- **Database scaling**: Azure SQL Database auto-scaling
- **Stateless services**: All services are stateless for easy scaling

### 7.3 Security
- **Authentication**: JWT tokens with refresh mechanism
- **Authorization**: Role-based access control
- **Data encryption**: HTTPS for all communications
- **Input validation**: Comprehensive validation on all inputs
- **SQL injection protection**: Parameterized queries only

### 7.4 Reliability
- **Uptime**: 99.9% availability target
- **Backup**: Daily automated database backups
- **Disaster recovery**: Cross-region backup strategy
- **Health checks**: Comprehensive health monitoring

### 7.5 Usability
- **Multilingual UI**: Support for English, Russian, Arabic
- **Responsive design**: Works on desktop, tablet, mobile
- **Accessibility**: WCAG 2.1 compliance
- **User experience**: Intuitive interface for library operations

## 8. API Design Standards

### 8.1 REST API Conventions
```
GET    /api/books           # Get all books (with pagination)
GET    /api/books/{id}      # Get specific book
POST   /api/books           # Create new book
PUT    /api/books/{id}      # Update entire book
PATCH  /api/books/{id}      # Partial update
DELETE /api/books/{id}      # Delete book
```

### 8.2 Response Formats
```json
// Success Response
{
  "success": true,
  "data": { ... },
  "message": "Operation completed successfully"
}

// Error Response
{
  "success": false,
  "errors": ["Error message 1", "Error message 2"],
  "message": "Operation failed"
}

// Paginated Response
{
  "success": true,
  "data": [...],
  "pagination": {
    "currentPage": 1,
    "totalPages": 10,
    "totalItems": 100,
    "pageSize": 10
  }
}
```

### 8.3 Authentication
```
Authorization: Bearer {JWT_TOKEN}
```

## 9. Development Roadmap

### 9.1 Phase 1: Foundation (Weeks 1-4)
- ✅ Project structure setup
- ✅ Shared libraries creation
- ✅ Book Catalog Service domain entities
- ⏳ Book Catalog Service repository interfaces
- ⏳ Book Catalog Service infrastructure (DbContext)
- ⏳ Book Catalog Service API controllers
- ⏳ Database migrations
- ⏳ Basic CRUD operations testing

### 9.2 Phase 2: Core Services (Weeks 5-8)
- User Management Service
- Basic authentication system
- Lending Service core functionality
- Service-to-service communication
- API Gateway setup

### 9.3 Phase 3: Advanced Features (Weeks 9-12)
- Notification Service
- Frontend web application
- Search functionality enhancement
- File upload for book covers
- Basic reporting

### 9.4 Phase 4: Production Ready (Weeks 13-16)
- Analytics Service
- Payment Service (if needed)
- Comprehensive testing
- Performance optimization
- Azure deployment
- CI/CD pipeline

## 10. Azure Infrastructure

### 10.1 Resource Group Structure
```
rg-shaieb-library-dev     # Development environment
rg-shaieb-library-prod    # Production environment
```

### 10.2 Azure Resources (Per Environment)
```
Resource Group: rg-shaieb-library-prod
├── App Services
│   ├── shaieb-gateway          (API Gateway)
│   ├── shaieb-book-catalog     (Book Catalog Service)
│   ├── shaieb-user-mgmt        (User Management Service)
│   ├── shaieb-lending          (Lending Service)
│   ├── shaieb-notifications    (Notification Service)
│   └── shaieb-web-app          (Frontend)
├── Databases
│   ├── shaieb-sql-server       (SQL Server)
│   ├── BookCatalogDb          (Book Catalog Database)
│   ├── UserManagementDb       (User Management Database)
│   ├── LendingDb              (Lending Database)
│   └── NotificationDb         (Notification Database)
├── Storage
│   ├── shaieblibrarystorage   (Blob Storage for files)
│   └── shaiebcontainerregistry (Container Registry)
├── Messaging
│   └── shaieb-servicebus      (Azure Service Bus)
├── Monitoring
│   ├── shaieb-app-insights    (Application Insights)
│   └── shaieb-log-analytics   (Log Analytics Workspace)
└── Security
    └── shaieb-keyvault        (Key Vault for secrets)
```

### 10.3 Networking
```
Virtual Network: shaieb-library-vnet
├── Subnet: app-services-subnet
├── Subnet: database-subnet
└── Subnet: gateway-subnet
```

## 11. Cost Estimates (Monthly)

### 11.1 Development Environment (~$35/month)
```
- App Service Plan (B1): $13 x 1 = $13
- Azure SQL Database (Basic): $5
- Storage Account (Hot): $2
- Application Insights: $2
- Service Bus (Basic): $0.05
- Container Registry: $5
- Total: ~$27/month
```

### 11.2 Production Environment (~$150/month)
```
- App Service Plan (S1): $73 x 2 = $146
- Azure SQL Database (S2): $30
- Storage Account (Hot): $5
- Application Insights: $10
- Service Bus (Standard): $10
- Container Registry: $5
- Key Vault: $1
- Total: ~$207/month
```

### 11.3 Scalable Production (~$300-500/month)
```
- Azure Kubernetes Service: $150
- Azure SQL Database (S4): $100
- Application Gateway: $20
- Redis Cache: $30
- Load Balancer: $18
- Enhanced monitoring: $20
- Total: ~$338/month
```

## 12. Current Progress

### 12.1 Completed
- ✅ Repository structure created
- ✅ Shared projects setup
- ✅ Book Catalog service projects created
- ✅ Project dependencies configured
- ✅ Domain entities created (Book, Author, Publisher, Category, BookAuthor, BookCategory)
- ✅ Enums defined (Language, BookStatus, AuthorRole)

### 12.2 Next Steps
- Create repository interfaces in Domain layer
- Implement DbContext in Infrastructure layer
- Create first migration
- Implement repository classes
- Create DTOs and services in Application layer
- Build API controllers
- Add validation and error handling
- Create unit tests

### 12.3 File Structure (Current)
```
ShaiebLibrary/
├── ShaiebLibrary.sln
├── .gitignore
├── README.md
├── src/
│   ├── services/
│   │   └── book-catalog/
│   │       ├── ShaiebLibrary.BookCatalog.API/
│   │       ├── ShaiebLibrary.BookCatalog.Application/
│   │       ├── ShaiebLibrary.BookCatalog.Domain/
│   │       │   ├── Entities/
│   │       │   │   ├── Book.cs
│   │       │   │   ├── Author.cs
│   │       │   │   ├── Publisher.cs
│   │       │   │   ├── Category.cs
│   │       │   │   ├── BookAuthor.cs
│   │       │   │   └── BookCategory.cs
│   │       │   └── Enums/
│   │       │       ├── Language.cs
│   │       │       ├── BookStatus.cs
│   │       │       └── AuthorRole.cs
│   │       ├── ShaiebLibrary.BookCatalog.Infrastructure/
│   │       └── ShaiebLibrary.BookCatalog.Tests/
│   └── shared/
│       ├── ShaiebLibrary.Shared.Domain/
│       ├── ShaiebLibrary.Shared.Application/
│       └── ShaiebLibrary.Shared.Infrastructure/
├── infrastructure/
└── docs/
```

## 13. Key Architectural Decisions

### 13.1 Monorepo vs Multi-repo
**Decision**: Monorepo
**Rationale**: Easier management for single developer, shared evolution, atomic changes

### 13.2 Architecture Pattern
**Decision**: Clean Architecture
**Rationale**: Better than CQRS for this domain complexity, simpler to understand and maintain

### 13.3 Database Strategy
**Decision**: SQL Server with Entity Framework Core
**Rationale**: Strong consistency requirements, relational data, mature tooling

### 13.4 API Gateway
**Decision**: Custom Ocelot Gateway
**Rationale**: Cost-effective (~$13/month vs $140+/month for APIM), sufficient for requirements

### 13.5 Deployment Strategy
**Decision**: Multiple App Services (one per microservice)
**Rationale**: Independent scaling, easier management, cost-effective for expected load

This document serves as the complete system specification and can be used to continue development from any point.
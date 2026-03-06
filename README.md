# WashBooking - Modular Monolith Architecture

**Kiến trúc:** Clean Architecture + Modular Monolith  
**Framework:** .NET 8.0 / .NET 9.0  
**Pattern:** CQRS + Domain-Driven Design (DDD)  
**Tình trạng:** 🚧 Đang xây dựng BuildingBlocks Foundation

---

## 📁 Cấu Trúc Dự Án Hiện Tại

```
WashBooking/
├── Directory.Build.props
├── WashBooking.sln
├── README.md (File này)
│
├── src/
│   ├── BuildingBlocks/              # ✅ Core Foundation Layer
│   │   ├── Domain/                  # ✅ HOÀN THÀNH
│   │   │   ├── Entity.cs
│   │   │   ├── ValueObject.cs
│   │   │   ├── IAggregateRoot.cs
│   │   │   ├── IAuditableEntity.cs
│   │   │   ├── IDomainEvent.cs
│   │   │   ├── IDomainEventHandler.cs
│   │   │   ├── Result.cs            # ✅ Result Pattern
│   │   │   ├── Result.ImplicitConverters.cs
│   │   │   ├── IResult.cs
│   │   │   ├── Error.cs
│   │   │   ├── ErrorType.cs
│   │   │   └── ValidationError.cs
│   │   │
│   │   ├── Application/             # ✅ HOÀN THÀNH
│   │   │   └── Abstractions/
│   │   │       ├── Messaging/       # ✅ CQRS Pattern
│   │   │       │   ├── ICommand.cs
│   │   │       │   ├── ICommandHandler.cs
│   │   │       │   ├── IQuery.cs
│   │   │       │   └── IQueryHandler.cs
│   │   │       │
│   │   │       ├── Behaviors/       # ✅ MediatR Pipeline
│   │   │       │   ├── ValidationPipelineBehavior.cs
│   │   │       │   └── LoggingPipelineBehavior.cs
│   │   │       │
│   │   │       ├── Data/
│   │   │       │   └── IUnitOfWork.cs
│   │   │       │
│   │   │       ├── Services/
│   │   │       │   └── IDateTimeProvider.cs
│   │   │       │
│   │   │       ├── Authentication/
│   │   │       │   ├── IPasswordHasher.cs
│   │   │       │   └── IUserContext.cs
│   │   │       │
│   │   │       ├── Query/           # ✅ Pagination Support
│   │   │       │   ├── IPagedQuery.cs
│   │   │       │   ├── PagedResult.cs
│   │   │       │   ├── PagedQueryExtensions.cs
│   │   │       │   └── SortOrder.cs
│   │   │       │
│   │   │       └── Notification/
│   │   │           └── Mail/
│   │   │               └── IMailService.cs
│   │   │
│   │   ├── Infrastructure/          # ✅ HOÀN THÀNH
│   │   │   ├── DependencyInjection/
│   │   │   │   └── DependencyInjection.cs  # ✅ Extension Methods
│   │   │   │
│   │   │   ├── Authentication/
│   │   │   │   ├── PasswordHasher.cs       # BCrypt
│   │   │   │   ├── UserContext.cs
│   │   │   │   └── ClaimsPrincipalExtensions.cs
│   │   │   │
│   │   │   └── Persistence/
│   │   │       ├── Database/
│   │   │       │   └── ApplicationDbContext.cs  # ✅ Abstract Base
│   │   │       │
│   │   │       ├── DomainEvents/
│   │   │       │   ├── DomainEventsDispatcher.cs
│   │   │       │   └── IDomainEventsDispatcher.cs
│   │   │       │
│   │   │       ├── Interceptors/
│   │   │       │   └── AuditableInterceptor.cs
│   │   │       │
│   │   │       └── Time/
│   │   │           └── DateTimeProvider.cs
│   │   │
│   │   ├── Presentation/            # 📂 EMPTY (Chưa cần)
│   │   └── BuildingBlocks.csproj
│   │
│   ├── Modules/                     # ⚠️ CHƯA CÓ MODULE NÀO
│   │   └── (Chưa tạo User Module)
│   │
│   └── WashBooking.Api/             # ✅ API Entry Point
│       ├── Program.cs               # ⚠️ Chưa cấu hình gì
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── WashBooking.Api.csproj
│
└── test/                            # ⚠️ CHƯA CÓ TESTS
```

---

## ✅ Những Gì ĐÃ CÓ (Đánh Giá)

### 🟢 **BuildingBlocks/Domain** - HOÀN THIỆN 95%

| Component | Status | Ghi Chú |
|-----------|--------|---------|
| `Entity.cs` | ✅ OK | Base class cho tất cả Entity, có Domain Events |
| `ValueObject.cs` | ✅ OK | Base class cho Value Objects (DDD) |
| `IAggregateRoot.cs` | ✅ OK | Marker interface cho Aggregate Root |
| `IAuditableEntity.cs` | ✅ OK | Interface cho CreatedOnUtc/ModifiedOnUtc |
| `IDomainEvent.cs` | ✅ OK | Marker interface cho Domain Events |
| `IDomainEventHandler.cs` | ✅ OK | Handler cho Domain Events |
| `Result.cs` | ✅ XUẤT SẮC | Result Pattern chuẩn (Railway Oriented) |
| `Result.ImplicitConverters.cs` | ✅ OK | Implicit operators cho Result |
| `IResult.cs` | ✅ OK | Interface cho Result pattern |
| `Error.cs` | ✅ OK | Error record với Factory Methods |
| `ErrorType.cs` | ✅ OK | Enum: Failure, NotFound, Validation, Conflict |
| `ValidationError.cs` | ✅ OK | Kế thừa Error, chứa list errors |

**Đánh giá:** Domain layer CỰC KỲ CHẮC CHẮN, không cần sửa gì thêm.

---

### 🟢 **BuildingBlocks/Application** - HOÀN THIỆN 98%

#### **CQRS Messaging**
| Component | Status | Ghi Chú |
|-----------|--------|---------|
| `ICommand.cs` | ✅ OK | Command pattern (returns `Result`) |
| `ICommand<TResponse>` | ✅ OK | Command with response (returns `Result<T>`) |
| `IQuery<TResponse>` | ✅ OK | Query pattern (returns `Result<T>`) |
| `ICommandHandler.cs` | ✅ OK | Handler cho Command |
| `IQueryHandler.cs` | ✅ OK | Handler cho Query |

#### **MediatR Behaviors**
| Component | Status | Ghi Chú |
|-----------|--------|---------|
| `ValidationPipelineBehavior.cs` | ✅ XUẤT SẮC | Auto-validate FluentValidation → Result |
| `LoggingPipelineBehavior.cs` | ✅ OK | Log request/response với Serilog |

#### **Cross-Cutting Concerns**
| Component | Status | Ghi Chú |
|-----------|--------|---------|
| `IUnitOfWork.cs` | ✅ OK | Interface cho SaveChangesAsync |
| `IDateTimeProvider.cs` | ✅ OK | Testable DateTime abstraction |
| `IPasswordHasher.cs` | ✅ OK | Interface cho BCrypt |
| `IUserContext.cs` | ✅ OK | Current user context |
| `IMailService.cs` | ⚠️ EMPTY | Chưa có implementation |

#### **Pagination**
| Component | Status | Ghi Chú |
|-----------|--------|---------|
| `PagedResult.cs` | ✅ XUẤT SẮC | Immutable paged result với Map() |
| `IPagedQuery.cs` | ✅ OK | Interface cho paging queries |
| `PagedQueryExtensions.cs` | ✅ OK | Extension methods cho IQueryable |
| `SortOrder.cs` | ✅ OK | Enum: Ascending/Descending |

**Đánh giá:** Application layer CỰC KỲ CHUẨN, chỉ thiếu Mail implementation.

---

### 🟢 **BuildingBlocks/Infrastructure** - HOÀN THIỆN 90%

#### **Dependency Injection**
| Component | Status | Ghi Chú |
|-----------|--------|---------|
| `DependencyInjection.cs` | ✅ XUẤT SẮC | Modular DI với Extension Methods |

**Methods có sẵn:**
- `AddBuildingBlocks()` - Đăng ký tất cả
- `AddMediatRPipelines()` - Validation + Logging behaviors
- `AddAuthenticationServices()` - PasswordHasher + UserContext
- `AddDateTimeProvider()` - DateTime abstraction
- `AddDomainEventDispatcher()` - Domain events dispatcher
- `AddDatabaseInterceptors()` - AuditableInterceptor
- `AddModuleDbContext<T>()` - DbContext với interceptors
- `AddMediatRHandlers()` - Auto-register handlers

#### **Authentication**
| Component | Status | Ghi Chú |
|-----------|--------|---------|
| `PasswordHasher.cs` | ✅ OK | BCrypt implementation |
| `UserContext.cs` | ✅ OK | Get current user từ HttpContext.User |
| `ClaimsPrincipalExtensions.cs` | ✅ OK | Helper methods |

#### **Persistence**
| Component | Status | Ghi Chú |
|-----------|--------|---------|
| `ApplicationDbContext.cs` | ✅ XUẤT SẮC | Abstract base cho module DbContext |
| `DomainEventsDispatcher.cs` | ✅ OK | Dispatch events qua MediatR |
| `AuditableInterceptor.cs` | ✅ OK | Auto-set CreatedOnUtc/ModifiedOnUtc |
| `DateTimeProvider.cs` | ✅ OK | Implementation của IDateTimeProvider |

**Đánh giá:** Infrastructure layer CHUẨN CHỈNH, chỉ thiếu UnitOfWork implementation.

---

## ⚠️ Những Gì CÒN THIẾU / CẦN SỬA

### 🔴 **CRITICAL - Thiếu Module**
- [ ] **User Module** chưa được tạo (Domain, Application, Infrastructure, Presentation)
- [ ] Folder `src/Modules/` đang EMPTY

### 🟡 **MEDIUM - Thiếu Implementation**
| Component | Vị Trí | Mức Độ |
|-----------|--------|--------|
| UnitOfWork.cs | `Infrastructure/Persistence/` | MEDIUM |
| IMailService implementation | `Infrastructure/Notification/` | LOW |
| Outbox Pattern | `Infrastructure/Persistence/Outbox/` | OPTIONAL |

### 🟡 **MEDIUM - API Chưa Cấu Hình**
- [ ] `Program.cs` chưa đăng ký BuildingBlocks
- [ ] Chưa có Connection String
- [ ] Chưa cấu hình Serilog
- [ ] Chưa cấu hình Authentication/Authorization
- [ ] Chưa có Exception Handling Middleware

### 🟢 **LOW - Testing**
- [ ] Chưa có Unit Tests
- [ ] Chưa có Integration Tests
- [ ] Folder `test/` đang EMPTY

---

## 🎯 RESULT PATTERN - Hướng Dẫn Sử Dụng Chi Tiết

### **1. Result (Không Giá Trị)**

```csharp
// ✅ SUCCESS
public Result DeleteUser(Guid userId)
{
    var user = _users.Find(userId);
    if (user is null)
        return Error.NotFound("User.NotFound", "User không tồn tại");
    
    _users.Remove(user);
    return Result.Success();
}

// Usage
var result = DeleteUser(userId);
if (result.IsSuccess)
{
    // Success logic
}
else
{
    // result.Error chứa thông tin lỗi
    Console.WriteLine(result.Error.Message);
}
```

### **2. Result<T> (Có Giá Trị)**

```csharp
// ✅ COMMAND HANDLER
public sealed class CreateUserCommandHandler 
    : ICommandHandler<CreateUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        // Kiểm tra duplicate email
        if (await _userRepository.ExistsByEmailAsync(command.Email))
        {
            return Error.Conflict(
                "User.DuplicateEmail", 
                $"Email '{command.Email}' đã tồn tại");
        }

        var user = User.Create(command.Email, command.Password);
        
        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id; // ✅ Implicit conversion từ Guid → Result<Guid>
    }
}
```

### **3. Sử Dụng Trong Controller**

```csharp
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    [HttpPost]
    public async Task<IActionResult> CreateUser(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(request.Email, request.Password);
        
        var result = await _sender.Send(command, cancellationToken);

        // ✅ MATCH PATTERN (Functional Style)
        return result.Match(
            onSuccess: userId => CreatedAtAction(
                nameof(GetUser), 
                new { id = userId }, 
                userId),
            onFailure: errors => errors[0].Type switch
            {
                ErrorType.NotFound => NotFound(errors),
                ErrorType.Validation => BadRequest(errors),
                ErrorType.Conflict => Conflict(errors),
                _ => StatusCode(500, errors)
            }
        );
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _sender.Send(query);

        // ✅ IF-ELSE PATTERN (Imperative Style)
        if (result.IsError)
        {
            return result.FirstError.Type switch
            {
                ErrorType.NotFound => NotFound(result.Errors),
                _ => StatusCode(500, result.Errors)
            };
        }

        return Ok(result.Value);
    }
}
```

### **4. FluentValidation + Result Pattern**

```csharp
// ✅ VALIDATOR
public sealed class CreateUserCommandValidator 
    : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email không hợp lệ");

        RuleFor(x => x.Password)
            .MinimumLength(8)
            .WithMessage("Mật khẩu phải ít nhất 8 ký tự");
    }
}

// ✅ AUTO VALIDATION (ValidationPipelineBehavior đã xử lý)
// Khi gọi _sender.Send(command), nếu validation fail
// → Tự động return Result.Failure(ValidationError)
```

### **5. Domain Errors (Best Practice)**

```csharp
// ✅ Domain/Users/UserErrors.cs
public static class UserErrors
{
    public static Error NotFound(Guid userId) => Error.NotFound(
        "User.NotFound",
        $"User với ID '{userId}' không tồn tại");

    public static Error DuplicateEmail(string email) => Error.Conflict(
        "User.DuplicateEmail",
        $"Email '{email}' đã được sử dụng");

    public static Error InvalidCredentials => Error.Validation(
        "User.InvalidCredentials",
        "Email hoặc mật khẩu không đúng");
}

// ✅ Sử dụng
return UserErrors.NotFound(userId);
return UserErrors.DuplicateEmail(command.Email);
```

---

## 🏗️ CẤU TRÚC MODULE (Template)

### **Module Chuẩn (Ví Dụ: User Module)**

```
src/Modules/Users/
├── Users.Domain/
│   ├── Entities/
│   │   └── User.cs
│   ├── ValueObjects/
│   │   └── Email.cs
│   ├── Events/
│   │   └── UserCreatedDomainEvent.cs
│   ├── Errors/
│   │   └── UserErrors.cs
│   └── Repositories/
│       └── IUserRepository.cs
│
├── Users.Application/
│   ├── Users/
│   │   ├── Commands/
│   │   │   ├── CreateUser/
│   │   │   │   ├── CreateUserCommand.cs
│   │   │   │   ├── CreateUserCommandHandler.cs
│   │   │   │   └── CreateUserCommandValidator.cs
│   │   │   └── UpdateUser/
│   │   │       ├── UpdateUserCommand.cs
│   │   │       └── UpdateUserCommandHandler.cs
│   │   │
│   │   └── Queries/
│   │       ├── GetUserById/
│   │       │   ├── GetUserByIdQuery.cs
│   │       │   └── GetUserByIdQueryHandler.cs
│   │       └── GetUsers/
│   │           ├── GetUsersQuery.cs
│   │           └── GetUsersQueryHandler.cs
│   │
│   ├── EventHandlers/
│   │   └── UserCreatedDomainEventHandler.cs
│   │
│   └── DependencyInjection.cs
│
├── Users.Infrastructure/
│   ├── Persistence/
│   │   ├── UsersDbContext.cs
│   │   ├── Configurations/
│   │   │   └── UserConfiguration.cs
│   │   ├── Repositories/
│   │   │   └── UserRepository.cs
│   │   └── Migrations/
│   │
│   └── DependencyInjection.cs
│
└── Users.Presentation/
    ├── Controllers/
    │   └── UsersController.cs
    ├── Requests/
    │   ├── CreateUserRequest.cs
    │   └── UpdateUserRequest.cs
    ├── Responses/
    │   └── UserResponse.cs
    └── DependencyInjection.cs
```

---

## 📝 CÁCH DÙNG BUILDINGBLOCKS TRONG MODULE

### **1. Users.Application/DependencyInjection.cs**

```csharp
using BuildingBlocks.Infrastructure.DependencyInjection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Users.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersApplication(
        this IServiceCollection services)
    {
        // ✅ Đăng ký MediatR handlers (tự động quét)
        services.AddMediatRHandlers(typeof(DependencyInjection).Assembly);

        // ✅ Đăng ký FluentValidation validators
        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly);

        return services;
    }
}
```

### **2. Users.Infrastructure/DependencyInjection.cs**

```csharp
using BuildingBlocks.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Domain.Repositories;
using Users.Infrastructure.Persistence;
using Users.Infrastructure.Persistence.Repositories;

namespace Users.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration
            .GetConnectionString("Database")!;

        // ✅ Đăng ký DbContext với BuildingBlocks interceptors
        services.AddModuleDbContext<UsersDbContext>(connectionString);

        // ✅ Đăng ký Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
```

### **3. Users.Infrastructure/Persistence/UsersDbContext.cs**

```csharp
using BuildingBlocks.Infrastructure.Persistence.Database;
using BuildingBlocks.Infrastructure.Persistence.DomainEvents;
using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;

namespace Users.Infrastructure.Persistence;

// ✅ Kế thừa từ ApplicationDbContext
public sealed class UsersDbContext(
    DbContextOptions<UsersDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher)
    : ApplicationDbContext(options, domainEventsDispatcher)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ✅ Áp dụng configurations
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(UsersDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
```

### **4. WashBooking.Api/Program.cs**

```csharp
using BuildingBlocks.Infrastructure.DependencyInjection;
using Serilog;
using Users.Application;
using Users.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ✅ 1. Đăng ký BuildingBlocks (Core)
builder.Services.AddBuildingBlocks();

// ✅ 2. Đăng ký Modules
builder.Services.AddUsersApplication();
builder.Services.AddUsersInfrastructure(builder.Configuration);

// ✅ 3. Đăng ký Controllers
builder.Services.AddControllers();

// ✅ 4. Cấu hình Serilog
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

// ✅ 5. Middleware Pipeline
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

---

## 🔧 DEPENDENCIES (BuildingBlocks.csproj)

```xml
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="FluentValidation" Version="12.1.0" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.10.0" />
<PackageReference Include="MediatR" Version="12.4.1" />
<PackageReference Include="MediatR.Contracts" Version="2.0.1" />
<PackageReference Include="Microsoft.AspNetCore.Http" Version="2.3.9" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.5" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.5" />
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.0" />
<PackageReference Include="Serilog" Version="4.1.0" />
```

---

## 🚀 ACTION ITEMS (Ưu Tiên)

### **Phase 1: Hoàn Thiện BuildingBlocks (OPTIONAL)**
- [ ] Tạo `UnitOfWork.cs` implementation (nếu muốn riêng khỏi DbContext)
- [ ] Implement `IMailService` (MailKit/SendGrid)

### **Phase 2: Tạo User Module (CRITICAL)**
- [ ] Tạo `Users.Domain` project
- [ ] Tạo `Users.Application` project
- [ ] Tạo `Users.Infrastructure` project
- [ ] Tạo `Users.Presentation` project
- [ ] Implement CRUD operations

### **Phase 3: Cấu Hình API**
- [ ] Cấu hình `Program.cs`
- [ ] Thêm Connection String
- [ ] Cấu hình Serilog
- [ ] Thêm Exception Handling Middleware
- [ ] Thêm Authentication/Authorization

### **Phase 4: Testing**
- [ ] Tạo Unit Tests cho Domain
- [ ] Tạo Integration Tests cho Application
- [ ] Tạo API Tests

---

## 📊 ĐÁNH GIÁ TỔNG QUAN

| Layer | Hoàn Thành | Chất Lượng | Ghi Chú |
|-------|------------|------------|---------|
| **BuildingBlocks.Domain** | 100% | ⭐⭐⭐⭐⭐ | XUẤT SẮC, không cần sửa |
| **BuildingBlocks.Application** | 98% | ⭐⭐⭐⭐⭐ | XUẤT SẮC, thiếu Mail impl |
| **BuildingBlocks.Infrastructure** | 95% | ⭐⭐⭐⭐⭐ | XUẤT SẮC, có thể thêm UoW |
| **Modules** | 0% | ❌ | CHƯA CÓ MODULE NÀO |
| **API** | 10% | ⭐ | Chỉ có skeleton |
| **Tests** | 0% | ❌ | CHƯA CÓ TESTS |

---

## ✅ KẾT LUẬN

**BuildingBlocks của bạn ĐÃ CỰC KỲ CHẮC CHẮN!**

✅ **Không cần sửa gì trong BuildingBlocks hiện tại**  
✅ **Result Pattern hoàn hảo**  
✅ **CQRS + MediatR chuẩn chỉnh**  
✅ **DI pattern modular rất tốt**  

**Bước tiếp theo:**
1. Tạo **User Module** theo template trên
2. Cấu hình **Program.cs**
3. Thêm **Connection String**
4. Chạy **Migration**
5. Test **CRUD operations**

---

**Tác giả:** BuildingBlocks Foundation  
**Ngày cập nhật:** 20/01/2026  
**Phiên bản:** 1.0.0


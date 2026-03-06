using System.Reflection;
using BuildingBlocks.Application.Abstractions.Authentication;
using BuildingBlocks.Application.Abstractions.Behaviors;
using BuildingBlocks.Application.Abstractions.Services;
using BuildingBlocks.Infrastructure.Authentication;
using BuildingBlocks.Infrastructure.Persistence.DomainEvents;
using BuildingBlocks.Infrastructure.Persistence.Interceptors;
using BuildingBlocks.Infrastructure.Persistence.Times;
using BuildingBlocks.Presentation.Infrastructures;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for registering BuildingBlocks infrastructure services.
/// Provides a centralized configuration for cross-cutting concerns across all modules.
/// </summary>
/// <remarks>
/// This class follows the Extension Method pattern for clean DI registration.
/// Each method is organized by feature area for better maintainability.
/// 
/// Usage in Program.cs or Startup.cs:
/// <code>
/// builder.Services.AddBuildingBlocks();
/// </code>
/// </remarks>
public static class BuildingBlocksServiceExtensions
{
    /// <summary>
    /// Registers all core BuildingBlocks services to the DI container.
    /// This is the main entry point for cross-cutting concern registration.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <remarks>
    /// This method orchestrates the registration of:
    /// - MediatR pipeline behaviors (Validation, Logging)
    /// - Authentication services (Password hashing, User context)
    /// - Date/Time abstraction for testability
    /// - Domain event dispatching
    /// - Database interceptors (Auditing, Soft delete, etc.)
    /// 
    /// Call this once in your application startup.
    /// </remarks>
    /// <example>
    /// <code>
    /// // In Program.cs
    /// var builder = WebApplication.CreateBuilder(args);
    /// builder.Services.AddBuildingBlocks();
    /// </code>
    /// </example>
    public static IServiceCollection AddBuildingBlocks(this IServiceCollection services, Assembly[] moduleAssemblies)
    {
        services.AddMediatR(config => 
        {
            // 1. Khởi tạo Core bằng cách quét các modules
            config.RegisterServicesFromAssemblies(moduleAssemblies);
        
            // 2. Gắn Pipeline Behaviors (thay thế cho cách AddScoped cũ)
            config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
            config.AddOpenBehavior(typeof(LoggingPipelineBehavior<,>));
        });
        
        services.AddValidatorsFromAssemblies(moduleAssemblies, ServiceLifetime.Scoped);
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddAuthenticationServices();
        services.AddDateTimeProvider();
        services.AddDomainEventDispatcher();
        services.AddDatabaseInterceptors();

        return services;
    }
    
    /// <summary>
    /// Registers authentication and authorization related services.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <remarks>
    /// Registered services:
    /// 
    /// 1. IHttpContextAccessor (Singleton)
    ///    - Provides access to the current HTTP context
    ///    - Required by UserContext to read claims from JWT tokens
    /// 
    /// 2. IPasswordHasher (Scoped)
    ///    - BCrypt-based password hashing
    ///    - Used for hashing passwords during registration
    ///    - Used for verifying passwords during login
    ///    - Configuration: WorkFactor = 12 (balance between security and performance)
    /// 
    /// 3. IUserContext (Scoped)
    ///    - Provides information about the currently authenticated user
    ///    - Reads UserId and other claims from JWT token
    ///    - Returns null if user is not authenticated
    /// 
    /// Security considerations:
    /// - Passwords are never stored in plain text
    /// - BCrypt includes automatic salt generation
    /// - Work factor can be increased as hardware improves
    /// </remarks>
    /// <example>
    /// <code>
    /// // Usage in a command handler:
    /// public class CreatePostHandler
    /// {
    ///     private readonly IUserContext _userContext;
    ///     
    ///     public async Task Handle(CreatePostCommand command)
    ///     {
    ///         var currentUserId = _userContext.UserId; // Get authenticated user
    ///         var post = Post.Create(command.Title, currentUserId);
    ///         // ...
    ///     }
    /// }
    /// 
    /// // Usage for password hashing:
    /// public class RegisterUserHandler
    /// {
    ///     private readonly IPasswordHasher _passwordHasher;
    ///     
    ///     public async Task Handle(RegisterCommand command)
    ///     {
    ///         var hashedPassword = _passwordHasher.Hash(command.Password);
    ///         var user = User.Create(command.Email, hashedPassword);
    ///         // ...
    ///     }
    /// }
    /// </code>
    /// </example>
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
    {
        // Register IHttpContextAccessor as Singleton (thread-safe, holds no state)
        // This is required by UserContext to access HTTP claims
        services.AddHttpContextAccessor();

        // Register Password Hasher (BCrypt implementation)
        // Scoped lifetime ensures consistent hashing within a request
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // Register User Context (extracts user info from JWT claims)
        // Scoped lifetime ensures user context is consistent within a request
        services.AddScoped<IUserContext, UserContext>();

        return services;
    }

    /// <summary>
    /// Registers the date/time provider abstraction for testable time-dependent logic.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <remarks>
    /// This abstraction allows you to:
    /// - Write testable code that depends on the current time
    /// - Mock time in unit tests (e.g., test expiration logic)
    /// - Ensure consistent timestamps across the application
    /// 
    /// Production: Returns DateTime.UtcNow / DateTimeOffset.UtcNow
    /// Testing: Can be mocked to return fixed or controllable time values
    /// 
    /// Lifetime: Singleton (stateless, thread-safe)
    /// 
    /// Always use IDateTimeProvider instead of DateTime.UtcNow directly.
    /// </remarks>
    /// <example>
    /// <code>
    /// // In production code:
    /// public class CreateOrderHandler
    /// {
    ///     private readonly IDateTimeProvider _dateTimeProvider;
    ///     
    ///     public async Task Handle(CreateOrderCommand command)
    ///     {
    ///         var order = Order.Create(
    ///             command.Items,
    ///             _dateTimeProvider.UtcNow); // ✅ Testable
    ///         // vs DateTime.UtcNow ❌ Not testable
    ///     }
    /// }
    /// 
    /// // In unit tests:
    /// var mockDateTimeProvider = new Mock&lt;IDateTimeProvider&gt;();
    /// mockDateTimeProvider.Setup(x => x.UtcNow)
    ///     .Returns(new DateTime(2024, 1, 1, 12, 0, 0));
    /// 
    /// var handler = new CreateOrderHandler(mockDateTimeProvider.Object);
    /// // Test with predictable time values
    /// </code>
    /// </example>
    public static IServiceCollection AddDateTimeProvider(this IServiceCollection services)
    {
        // Singleton: No state, thread-safe, can be shared across all requests
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }

    /// <summary>
    /// Registers the domain events dispatcher for publishing domain events after SaveChanges.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <remarks>
    /// The domain events dispatcher follows the Domain Event pattern from DDD:
    /// 
    /// Flow:
    /// 1. Aggregate root raises domain events (e.g., UserCreatedEvent)
    /// 2. Events are stored in the aggregate's DomainEvents collection
    /// 3. When SaveChangesAsync is called, dispatcher:
    ///    a. Collects all domain events from tracked entities
    ///    b. Clears the events from entities
    ///    c. Publishes events via MediatR
    /// 4. Domain event handlers process the events (side effects)
    /// 
    /// This ensures:
    /// - Events are only published after successful database commit
    /// - Transactional consistency (events + data saved atomically)
    /// - Loose coupling between aggregates
    /// 
    /// Lifetime: Scoped (one instance per request/transaction)
    /// </remarks>
    /// <example>
    /// <code>
    /// // In an aggregate:
    /// public class User : Entity&lt;UserId&gt;, IAggregateRoot
    /// {
    ///     public static User Create(string email, string password)
    ///     {
    ///         var user = new User { Email = email };
    ///         user.RaiseDomainEvent(new UserCreatedEvent(user.Id, email));
    ///         return user;
    ///     }
    /// }
    /// 
    /// // In a domain event handler:
    /// public class UserCreatedEventHandler : INotificationHandler&lt;UserCreatedEvent&gt;
    /// {
    ///     public async Task Handle(UserCreatedEvent notification, CancellationToken ct)
    ///     {
    ///         // Send welcome email
    ///         // Create default settings
    ///         // Log analytics event
    ///     }
    /// }
    /// 
    /// // In SaveChangesAsync (called automatically):
    /// await _domainEventsDispatcher.DispatchAndClearEvents(dbContext, ct);
    /// </code>
    /// </example>
    public static IServiceCollection AddDomainEventDispatcher(this IServiceCollection services)
    {
        // Scoped: Each request gets its own dispatcher for transaction isolation
        services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();

        return services;
    }

    /// <summary>
    /// Registers EF Core interceptors that execute around database operations.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <remarks>
    /// Interceptors are EF Core's hook points for cross-cutting concerns.
    /// They execute automatically before/after SaveChanges.
    /// 
    /// Currently registered interceptors:
    /// 
    /// 1. AuditableInterceptor
    ///    - Automatically sets CreatedOnUtc for new entities
    ///    - Automatically sets ModifiedOnUtc for modified entities
    ///    - Only applies to entities implementing IAuditableEntity
    ///    - Ensures consistent audit trails without manual code
    /// 
    /// Future interceptors could include:
    /// - SoftDeleteInterceptor: Marks entities as deleted instead of removing them
    /// - TenantInterceptor: Automatically filters queries by tenant ID
    /// - PublishInterceptor: Publishes integration events after successful commit
    /// - PerformanceInterceptor: Logs slow queries for optimization
    /// 
    /// Lifetime: Singleton (stateless, thread-safe, no per-request state)
    /// </remarks>
    /// <example>
    /// <code>
    /// // Entity implementing IAuditableEntity:
    /// public class User : Entity&lt;UserId&gt;, IAuditableEntity
    /// {
    ///     public DateTime CreatedOnUtc { get; set; } // Auto-set on insert
    ///     public DateTime? ModifiedOnUtc { get; set; } // Auto-set on update
    /// }
    /// 
    /// // No manual code needed - interceptor handles it:
    /// var user = User.Create(email, password);
    /// await _repository.AddAsync(user);
    /// await _unitOfWork.SaveChangesAsync();
    /// 
    /// // CreatedOnUtc is automatically set to DateTime.UtcNow
    /// // ModifiedOnUtc is null (not modified yet)
    /// 
    /// user.UpdateEmail(newEmail);
    /// await _unitOfWork.SaveChangesAsync();
    /// 
    /// // ModifiedOnUtc is automatically updated to DateTime.UtcNow
    /// </code>
    /// </example>
    public static IServiceCollection AddDatabaseInterceptors(this IServiceCollection services)
    {
        // Singleton: Interceptors are stateless and can be shared across all requests
        // They don't hold any request-specific data
        services.AddSingleton<ISaveChangesInterceptor, AuditableInterceptor>();
        return services;
    }

    /// <summary>
    /// Registers a module's DbContext with a custom configuration action.
    /// Provides flexibility for multiple database providers (PostgreSQL, SQL Server, etc.)
    /// </summary>
    public static IServiceCollection AddModuleDbContext<TContext>(
        this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> configure)
        where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(configure);

        services.AddDbContext<TContext>((serviceProvider, options) =>
        {
            // Cho phép thằng Module tự quyết định nó xài Postgres hay SQL Server
            configure(serviceProvider, options);

            // BuildingBlocks chỉ đứng ra lo vụ nhét Interceptor vô thôi
            var interceptors = serviceProvider.GetServices<ISaveChangesInterceptor>();
            options.AddInterceptors(interceptors);
        });

        return services;
    }
    
}
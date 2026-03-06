using System.Reflection;
using BuildingBlocks.Domain;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace BuildingBlocks.Application.Abstractions.Behaviors;

public sealed class ValidationPipelineBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private static readonly Func<Error, TResponse>? FailureDelegate = CreateFailureDelegate();

    private static Func<Error, TResponse>? CreateFailureDelegate()
    {
        if (typeof(TResponse).IsGenericType && 
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            Type resultType = typeof(TResponse).GetGenericArguments()[0];
            MethodInfo? failureMethod = typeof(Result<>)
                .MakeGenericType(resultType)
                .GetMethod(nameof(Result<object>.Failure), [typeof(Error)]);

            if (failureMethod is not null)
            {
                // Ép thẳng thành Func<Error, TResponse> chạy tốc độ bàn thờ
                return (Func<Error, TResponse>)Delegate.CreateDelegate(typeof(Func<Error, TResponse>), failureMethod);
            }
        }
        return null;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);
        
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var validationFailures = validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .ToArray();

        if (validationFailures.Length == 0)
        {
            return await next();
        }
        
        ValidationError validationError = CreateValidationError(validationFailures);
        
        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(validationError);
        }

        if (FailureDelegate is not null)
        {
            return FailureDelegate(validationError);
        }

        throw new ValidationException(validationFailures);
    }

    private static ValidationError CreateValidationError(ValidationFailure[] validationFailures)
    {
        var errors = validationFailures.Select(f => 
            new Error(
                f.PropertyName,     
                f.ErrorMessage,      
                ErrorType.Validation
            )) 
            .ToArray();
        
        return new ValidationError(errors);
    }
}

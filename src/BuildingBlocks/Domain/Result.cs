using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace BuildingBlocks.Domain;

/// <summary>
/// Represents the success result of an operation, typically used to indicate that a process has completed successfully without returning a specific value or data.
/// </summary>
public readonly record struct Success;

/// <summary>
/// Represents a class containing utility methods for handling operation results.
/// </summary>
public readonly record struct Result : IResult
{
    private readonly Error _error;

    // Constructor private để ép dùng Factory Method
    private Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        _error = error;
    }

    public bool IsSuccess { get; }
    
    // Implement IResult
    public bool IsError => !IsSuccess;
    public IReadOnlyList<Error>? Errors => IsSuccess ? null : [_error];
    public Error Error => IsSuccess ? Error.None : _error;

    // 1. Logic Success (Tĩnh)
    public static Result Success() => new(true, Error.None);

    // 2. Logic Failure (Tĩnh) 
    public static Result Failure(Error error) => new(false, error);

    // Implicit conversion (Optional)
    [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Project is C# only")]
    public static implicit operator Result(Error error) => Failure(error);
}

public readonly partial record struct Result<TValue> : IResult<TValue>
    where TValue : notnull
{
    private readonly TValue? _value;

    private readonly ImmutableArray<Error>? _errors;
    
    private Result(TValue value)
    {
        _value = value;
        _errors = null;
    }

    private Result(Error error)
    {
        _value = default;
        _errors = [error];
    }

    private Result(List<Error> errors)
    {
		ArgumentNullException.ThrowIfNull(errors);

		if (errors.Count == 0)
        {
            throw new ArgumentException("Cannot create an Result<TValue> from an empty collection of errors. Provide at least one error.", nameof(errors));
        }

        _errors = errors.ToImmutableArray();
    }

    /// <summary>
    /// Gets the collection of errors.
    /// </summary>
    public IReadOnlyList<Error> Errors => _errors ?? [];
    
    /// <summary>
    /// Gets a value indicating whether the state is a success.
    /// </summary>
    public bool IsSuccess => _errors is null || _errors.Value.IsEmpty;

    /// <summary>
    /// Gets a value indicating whether the state is error.
    /// </summary>
    public bool IsError => !IsSuccess;
    
    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no value is present.</exception>
    public TValue Value
    {
        get
        {
            if (IsError)
            {
                throw new InvalidOperationException("The Value property cannot be accessed when Errors property is not empty. Check IsSuccess or IsError before accessing the Value.");
            }

            return _value!;
        }
    }

    /// <summary>
    /// Gets the first error.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no errors are present.</exception>
    public Error FirstError
    {
        get
        {
            if (IsSuccess || _errors is null || _errors.Value.IsEmpty)
            {
                throw new InvalidOperationException("The FirstError property cannot be accessed when Errors property is empty. Check IsError before accessing FirstError.");
            }

            return _errors.Value[0];
        }
    }
    
    [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Needed for clean API and Reflection support.")]
    public static Result<TValue> Failure(Error error) => new(error);
    
    [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Needed for clean API and Reflection support.")]
    public static Result<TValue> Success(TValue value) => new(value);
    
    /// <summary>
    /// Matches the result to one of two functions based on whether it represents success or failure.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value.</typeparam>
    /// <param name="onSuccess">Function to execute if the result is successful.</param>
    /// <param name="onFailure">Function to execute if the result contains errors.</param>
    /// <returns>The result of the executed function.</returns>
    /// <exception cref="ArgumentNullException">Thrown when onSuccess or onFailure is null.</exception>
    public TResult Match<TResult>(
        Func<TValue, TResult> onSuccess,
        Func<IReadOnlyList<Error>, TResult> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);
        
        return IsSuccess ? onSuccess(Value) : onFailure(Errors);
    }
}

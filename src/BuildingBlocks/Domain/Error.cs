using System.Diagnostics.CodeAnalysis;

namespace BuildingBlocks.Domain;

[SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification="System is C# only")]
public record Error // 👈 Dùng record class để hỗ trợ kế thừa
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static readonly Error NullValue = new("Error.NullValue", "Null value was provided", ErrorType.Failure);

    public Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }

    // Factory methods
    public static Error Failure(string code, string message) => 
        new(code, message, ErrorType.Failure);

    public static Error NotFound(string code, string message) => 
        new(code, message, ErrorType.NotFound);
    
    public static Error Validation(string code, string message) => 
        new(code, message, ErrorType.Validation); // Cái này cho lỗi đơn lẻ

    public static Error Conflict(string code, string message) => 
        new(code, message, ErrorType.Conflict);
        
    // Hàm này giúp tạo Error dạng ProblemDetails (dùng trong ValidationPipeline)
    public static Error Problem(string code, string message) =>
        new(code, message, ErrorType.Validation);
}

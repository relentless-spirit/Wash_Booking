using WashBooking.Domain.Common;

namespace WashBooking.Domain.Common
{
    /// <summary>
    /// Lớp kết quả để đóng gói một thao tác thành công hoặc thất bại.
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public IReadOnlyCollection<Error> Errors { get; }
        public Error Error => Errors.FirstOrDefault() ?? Error.None; // Lấy lỗi đầu tiên để tương thích

        protected Result(bool isSuccess, IReadOnlyCollection<Error> errors)
        {
            if (isSuccess && errors.Any(e => e != Error.None))
            {
                throw new InvalidOperationException("Không thể tạo kết quả thành công với một lỗi.");
            }
            if (!isSuccess && !errors.Any())
            {
                throw new InvalidOperationException("Kết quả thất bại phải có ít nhất một lỗi.");
            }

            IsSuccess = isSuccess;
            Errors = errors;
        }

        public static Result Success() => new(true, new[] { Error.None });

        // Phương thức cũ, dùng cho một lỗi duy nhất
        public static Result Failure(Error error) => new(false, new[] { error });

        // Phương thức mới bạn đề xuất, dùng cho danh sách lỗi
        public static Result Failure(IReadOnlyCollection<Error> errors) => new(false, errors);
    }

    /// <summary>
    /// Lớp kết quả có chứa một giá trị trả về khi thành công.
    /// </summary>
    public class Result<TValue> : Result
    {
        private readonly TValue? _value;

        public TValue Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("Không thể truy cập giá trị của một kết quả thất bại.");

        protected Result(TValue value) : base(true, new[] { Error.None })
        {
            _value = value;
        }

        protected Result(IReadOnlyCollection<Error> errors) : base(false, errors)
        {
        }

        public static Result<TValue> Success(TValue value) => new(value);

        public new static Result<TValue> Failure(Error error) => new(new[] { error });

        public static Result<TValue> Failure(IReadOnlyCollection<Error> errors) => new(errors);
    }
}
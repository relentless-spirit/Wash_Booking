namespace WashBooking.Domain.Common
{
    /// <summary>
    /// Đại diện cho một lỗi nghiệp vụ cụ thể.
    /// </summary>
    /// <param name="Code">Mã lỗi duy nhất để có thể xử lý trong code.</param>
    /// <param name="Message">Thông báo lỗi thân thiện với người dùng.</param>
    public record Error(string Code, string Message)
    {
        /// <summary>
        /// Một đối tượng Error tĩnh để đại diện cho trạng thái không có lỗi.
        /// </summary>
        public static readonly Error None = new(string.Empty, string.Empty);
    }
}

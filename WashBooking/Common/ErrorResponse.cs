namespace WashBooking.Common
{
    /// <summary>
    /// Đại diện cho một cấu trúc response lỗi chuẩn cho API.
    /// </summary>
    /// <param name="Code">Mã lỗi nội bộ.</param>
    /// <param name="Message">Thông báo lỗi thân thiện với người dùng.</param>
    public record ErrorResponse(string Code, string Message);
}
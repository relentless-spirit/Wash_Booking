namespace WashBooking.Application.Common.Settings;

public static class BookingCodeGenerator
{
    public static string Generate(Guid bookingId, DateTime dateTime)
    {
        const string prefix = "WB";
        var datePart = dateTime.ToString("yyyyMMdd");
        // Lấy 4 ký tự cuối của bookingId để đảm bảo mã gắn với chính ID đó
        var idPart = bookingId.ToString("N")[^4..].ToUpper();
        return $"{prefix}{datePart}-{idPart}";
    }
}
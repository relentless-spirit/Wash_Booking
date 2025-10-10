using WashBooking.Domain.Entities;

namespace WashBooking.Domain.Interfaces.Repositories;

public interface IBookingDetailRepository : IGenericRepository<BookingDetail>
{
    Task<List<BookingDetail>> GetScheduledBookingsInTimeRangeAsync(DateTime newStartTime, DateTime newEndTime);
    /// <summary>
    /// Lấy tất cả các công việc đã được xếp lịch trong một ngày cụ thể.
    /// </summary>
    Task<List<BookingDetail>> GetScheduledBookingsForDateAsync(DateOnly date);
}
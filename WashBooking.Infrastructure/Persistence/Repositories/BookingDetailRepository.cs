using Microsoft.EntityFrameworkCore;
using WashBooking.Domain.Entities;
using WashBooking.Domain.Enums;
using WashBooking.Domain.Interfaces.Repositories;

namespace WashBooking.Infrastructure.Persistence.Repositories;

public class BookingDetailRepository : GenericRepository<BookingDetail>, IBookingDetailRepository
{
    public BookingDetailRepository (MotoBikeWashingBookingContext context) : base(context)
    {
    }

    public async Task<List<BookingDetail>> GetScheduledBookingsInTimeRangeAsync(DateTime newStartTime, DateTime newEndTime)
    {
        return await _dbSet
            .Where(bd => bd.Status.Equals("Scheduled") || bd.Status.Equals("ServiceInProgress"))
            .Where(bd => newStartTime <= bd.PlannedEndTime && newEndTime >= bd.PlannedStartTime)
            .ToListAsync();
    }

    public async Task<List<BookingDetail>> GetScheduledBookingsForDateAsync(DateOnly date)
    {
        // 1. Chuyển DateOnly thành DateTime
        var startDateUnspecified = date.ToDateTime(TimeOnly.MinValue);
            
        // 2. "DÁN NHÃN" UTC CHO NÓ
        var startDateUtc = DateTime.SpecifyKind(startDateUnspecified, DateTimeKind.Utc);
        var endDateUtc = startDateUtc.AddDays(1);

        return await _dbSet
            // Include các dữ liệu liên quan mà bạn sẽ cần ở tầng Service
            .Include(bd => bd.Assignee) 
            .Include(bd => bd.Booking)
        
            // Chỉ quan tâm đến các công việc đã được xếp lịch, chưa hoàn thành hoặc hủy
            // Dùng == thay cho .Equals() sẽ được dịch sang SQL tốt hơn
            .Where(bd => bd.Status == BookingStatus.Scheduled || bd.Status == BookingStatus.ServiceInProgress) 
        
            // SỬA Ở ĐÂY: Lọc tất cả các booking có thời gian bắt đầu NẰM TRONG NGÀY ĐÓ
            .Where(bd => bd.PlannedStartTime >= startDateUtc && bd.PlannedStartTime < endDateUtc)
        
            .AsNoTracking()
            .ToListAsync();
    }
}
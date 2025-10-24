using Microsoft.EntityFrameworkCore;
using WashBooking.Domain.Entities;
using WashBooking.Domain.Interfaces.Repositories;

namespace WashBooking.Infrastructure.Persistence.Repositories;

public class BookingRepository : GenericRepository<Booking>, IBookingRepository
{
    public BookingRepository(MotoBikeWashingBookingContext context) : base(context)
    {
    }

    public async Task<List<Booking>> GetAllBookingForAdminAsync()
    {
        return await _dbSet
            .Include(b => b.BookingDetails)
            .ThenInclude(bd => bd.Service)
            .ToListAsync();
    }

    public async Task<Booking?> GetBookingByBookingCodeAsync(string bookingCode)
    {
        return await _dbSet
            .Include(b => b.BookingDetails)
            .ThenInclude(bd => bd.Service)
            .Include(b => b.BookingDetails)
            .ThenInclude(bd => bd.Assignee)
            .Include(b => b.BookingDetails)
            .ThenInclude(bd => bd.BookingDetailProgresses)
            .SingleOrDefaultAsync(b => b.BookingCode == bookingCode);
    }

    public async Task<List<Booking>> GetAllBookingByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(b => b.UserProfileId.Equals(userId))
            .Include(b => b.BookingDetails)
            .ThenInclude(bd => bd.Service)
            .Include(b => b.BookingDetails)
            .ThenInclude(bd => bd.Assignee)
            .ToListAsync();
    }
    
    public async Task<Booking?> GetAllInfoBookingByIdAsync(Guid id)
    {
        return await _dbSet
            // === CÁCH VIẾT GỌN GÀNG VÀ CHÍNH XÁC ===

            // 1. Chỉ cần Include một lần cho BookingDetails
            .Include(b => b.BookingDetails)
            // 2. Sau đó, "nối chuỗi" các ThenInclude từ nó
            .ThenInclude(bd => bd.Service)
            .Include(b => b.BookingDetails)
            .ThenInclude(bd => bd.Assignee)
            .Include(b => b.BookingDetails)
            // 3. Sửa lại đúng tên thuộc tính là ProgressHistory
            .ThenInclude(bd => bd.BookingDetailProgresses) 
            .AsNoTracking() // Tối ưu hóa cho truy vấn chỉ đọc
            .SingleOrDefaultAsync(b => b.Id == id);
    }
}
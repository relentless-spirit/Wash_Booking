using WashBooking.Domain.Entities;

namespace WashBooking.Domain.Interfaces.Repositories;

public interface IBookingRepository : IGenericRepository<Booking>
{
    Task<List<Booking>> GetAllBookingForAdminAsync();
    Task<Booking> GetBookingByBookingCodeAsync(string bookingCode);
    Task<List<Booking>> GetAllBookingByUserIdAsync(Guid userId);
    Task<Booking> GetAllInfoBookingByIdAsync(Guid id);
}
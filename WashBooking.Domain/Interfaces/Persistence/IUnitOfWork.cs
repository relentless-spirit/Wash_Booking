using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WashBooking.Domain.Entities;
using WashBooking.Domain.Interfaces.Repositories;

namespace WashBooking.Domain.Interfaces.Persistence
{
    public interface IUnitOfWork
    {
        // Add repository interfaces here, for example:
        IAccountRepository AccountRepository { get; }
        IBookingRepository BookingRepository { get; }
        IBookingDetailRepository BookingDetailRepository { get; }
        IGenericRepository<BookingDetailProgress> BookingDetailProgressRepository { get; }
        IGenericRepository<OauthAccount> OauthAccountRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        IGenericRepository<Payment> PaymentRepository { get; }
        IGenericRepository<Service> ServiceRepository { get; }
        IUserProfileRepository UserProfileRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}

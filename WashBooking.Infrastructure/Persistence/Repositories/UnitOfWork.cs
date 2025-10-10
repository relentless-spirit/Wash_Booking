using System;
using System.Threading.Tasks;
using WashBooking.Domain.Entities;
using WashBooking.Domain.Interfaces.Persistence;
using WashBooking.Domain.Interfaces.Repositories;

namespace WashBooking.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly MotoBikeWashingBookingContext _context;

        private IAccountRepository? _accountRepository;
        private IBookingRepository? _bookingRepository;
        private IBookingDetailRepository? _bookingDetailRepository;
        private IGenericRepository<BookingDetailProgress>? _bookingDetailProgressRepository;
        private IGenericRepository<OauthAccount>? _oauthAccountRepository;
        private IRefreshTokenRepository? _refreshTokenRepository;
        private IGenericRepository<Payment>? _paymentRepository;
        private IGenericRepository<Service>? _serviceRepository;
        private IUserProfileRepository? _userProfileRepository;

        public UnitOfWork(MotoBikeWashingBookingContext context)
        {
            _context = context;
        }

        public IAccountRepository AccountRepository => 
            _accountRepository ??= new AccountRepository(_context);

        public IBookingRepository BookingRepository => 
            _bookingRepository ??= new BookingRepository(_context);

        public IBookingDetailRepository BookingDetailRepository => 
            _bookingDetailRepository ??= new BookingDetailRepository(_context);

        public IGenericRepository<BookingDetailProgress> BookingDetailProgressRepository => 
            _bookingDetailProgressRepository ??= new GenericRepository<BookingDetailProgress>(_context);

        public IGenericRepository<OauthAccount> OauthAccountRepository => 
            _oauthAccountRepository ??= new GenericRepository<OauthAccount>(_context);

        public IRefreshTokenRepository RefreshTokenRepository => 
            _refreshTokenRepository ??= new RefreshTokenRepository(_context);

        public IGenericRepository<Payment> PaymentRepository => 
            _paymentRepository ??= new GenericRepository<Payment>(_context);

        public IGenericRepository<Service> ServiceRepository => 
            _serviceRepository ??= new GenericRepository<Service>(_context);

        public IUserProfileRepository UserProfileRepository => 
            _userProfileRepository ??= new UserProfileRepository(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                    _accountRepository = null;
                    _bookingRepository = null;
                    _bookingDetailRepository = null;
                    _bookingDetailProgressRepository = null;
                    _oauthAccountRepository = null;
                    _refreshTokenRepository = null;
                    _paymentRepository = null;
                    _serviceRepository = null;
                    _userProfileRepository = null;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

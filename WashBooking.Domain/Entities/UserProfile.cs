using System;
using System.Collections.Generic;
using WashBooking.Domain.Enums;

namespace WashBooking.Domain.Entities;

public partial class UserProfile
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = null!;

    public string? Phone { get; set; }

    public string Email { get; set; } = null!;

    public string? Address { get; set; }

    public Role Role { get; set; } = Role.Customer;

    public bool IsActive { get; set; } = true;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<BookingDetailProgress> BookingDetailProgresses { get; set; } = new List<BookingDetailProgress>();

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

using System;
using System.Collections.Generic;
using WashBooking.Domain.Enums;

namespace WashBooking.Domain.Entities;

public partial class Booking
{
    public Guid Id { get; set; }

    public Guid? UserProfileId { get; set; }

    public string BookingCode { get; set; } = null!;

    public string CustomerName { get; set; } = null!;

    public string CustomerPhone { get; set; } = null!;

    public string CustomerEmail { get; set; } = null!;

    public DateTime BookingDatetime { get; set; }

    public BookingStatus Status { get; set; }

    public decimal TotalAmount { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public string? PaymentMethod { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual UserProfile? UserProfile { get; set; }
}

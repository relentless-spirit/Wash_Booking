using System;
using System.Collections.Generic;

namespace WashBooking.Domain.Entities;

public partial class Payment
{
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string? TransactionId { get; set; }

    public string Status { get; set; } = null!;

    public string? PaymentDetails { get; set; }

    public DateTime? CreatedAt { get; set; }
}

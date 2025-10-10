using System;
using System.Collections.Generic;
using WashBooking.Domain.Enums;

namespace WashBooking.Domain.Entities;

public partial class BookingDetailProgress
{
    public Guid Id { get; set; }

    public Guid BookingDetailId { get; set; }

    public BookingStatus Status { get; set; }

    public string? Note { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual BookingDetail BookingDetail { get; set; } = null!;

    public virtual UserProfile? CreatedByUser { get; set; }
}

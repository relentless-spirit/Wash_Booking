using System;
using System.Collections.Generic;
using WashBooking.Domain.Enums;

namespace WashBooking.Domain.Entities;

public partial class BookingDetail
{
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    public Guid ServiceId { get; set; }

    public string VehicleDescription { get; set; } = null!;

    public Guid? AssigneeId { get; set; }

    public BookingStatus Status { get; set; }

    public int DurationMinutes { get; set; }

    public decimal Price { get; set; }

    public DateTime PlannedStartTime { get; set; }

    public DateTime PlannedEndTime { get; set; }

    public DateTime? ActualStartTime { get; set; }

    public DateTime? ActualEndTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual UserProfile? Assignee { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual ICollection<BookingDetailProgress> BookingDetailProgresses { get; set; } = new List<BookingDetailProgress>();

    public virtual Service Service { get; set; } = null!;
}

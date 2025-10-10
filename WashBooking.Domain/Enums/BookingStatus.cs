using System.ComponentModel;

namespace WashBooking.Domain.Enums;

public enum BookingStatus
{
    [Description("Scheduled, awaiting check-in")]
    Scheduled,

    [Description("Vehicle checked in")]
    CheckedIn,

    [Description("Service in progress")]
    ServiceInProgress,

    [Description("Under quality inspection")]
    QualityCheck,

    [Description("Ready for pickup")]
    ReadyForPickup,

    [Description("Completed")]
    Completed,

    [Description("Issue reported")]
    IssueReported,

    [Description("Cancelled")]
    Cancelled,

}
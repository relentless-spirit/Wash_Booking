using System;
using System.Collections.Generic;

namespace WashBooking.Domain.Entities;

public partial class OauthAccount
{
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }

    public string Provider { get; set; } = null!;

    public string? RefreshToken { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Account Account { get; set; } = null!;
}

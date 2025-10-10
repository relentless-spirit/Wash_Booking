using System;
using System.Collections.Generic;

namespace WashBooking.Domain.Entities;

public partial class RefreshToken
{
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool IsRevoked { get; set; }

    public virtual Account Account { get; set; } = null!;
}

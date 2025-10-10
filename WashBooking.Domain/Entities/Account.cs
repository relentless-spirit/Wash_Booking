using System;
using System.Collections.Generic;
using WashBooking.Domain.Enums;

namespace WashBooking.Domain.Entities;

public partial class Account
{
    public Guid Id { get; set; }

    public Guid UserProfileId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public AccountType AccountType { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<OauthAccount> OauthAccounts { get; set; } = new List<OauthAccount>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual UserProfile UserProfile { get; set; } = null!;
}

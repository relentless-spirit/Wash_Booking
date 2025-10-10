using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WashBooking.Application.Common.Settings
{
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings"; // Hằng số để tránh lỗi chính tả

        public string SecretKey { get; init; } = string.Empty;
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        public int TokenExpirationInMinutes { get; init; }
        public int RefreshTokenExpirationInDays { get; init; }
    }
}

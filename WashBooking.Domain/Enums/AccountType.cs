using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WashBooking.Domain.Enums
{
    public enum AccountType
    {
        /// <summary>
        /// Tài khoản được xác thực bằng mật khẩu do hệ thống quản lý.
        /// </summary>
        Local,

        /// <summary>
        /// Tài khoản được liên kết và xác thực qua Google.
        /// </summary>
        Google,
    }
}

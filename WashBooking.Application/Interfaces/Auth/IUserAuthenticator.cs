using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WashBooking.Application.DTOs.AuthDTO.LoginDTO;
using WashBooking.Domain.Entities;

namespace WashBooking.Application.Interfaces.Auth
{
    public interface IUserAuthenticator
    {
        Task<Account?> AuthenticateAsync(string usernameOrEmail, string password);
    }
}

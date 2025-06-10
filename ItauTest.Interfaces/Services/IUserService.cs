using ItauTest.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItauTest.Interfaces.Services
{
    public interface IUserService
    {
        Task<Usuario> GetUserByEmailAsync(string email);
        PasswordVerificationResult VerifyPassword(Usuario usuario, string password);
    }
}

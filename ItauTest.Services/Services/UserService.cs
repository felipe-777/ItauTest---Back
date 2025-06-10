using ItauTest.Data;
using ItauTest.Interfaces.Services;
using ItauTest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItauTest.Services.Services
{
    public class UserService : IUserService
    {
        private readonly ItauDbContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public UserService(ItauDbContext context, IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<Usuario> GetUserByEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        public PasswordVerificationResult VerifyPassword(Usuario usuario, string password)
        {
            var hasher = new PasswordHasher<Usuario>();
            return hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, password);
        }
    }
}

using ItauTest.Data;
using ItauTest.Interfaces.Services;
using ItauTest.Models;
using ItauTest.Services.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ItauTest.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<ItauDbContext> _contextMock;
        private readonly UserService _userService;
        private readonly Mock<IPasswordHasher<Usuario>> _passwordHasherMock;

        public UserServiceTests()
        {
            _contextMock = new Mock<ItauDbContext>();
            _passwordHasherMock = new Mock<IPasswordHasher<Usuario>>();
            _userService = new UserService(_contextMock.Object, _passwordHasherMock.Object);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ExistingEmail_ReturnsUser()
        {
            // Arrange
            var email = "test@example.com";
            var usuario = new Usuario { Email = email, SenhaHash = "hashedpassword" };
            var usuarios = new List<Usuario> { usuario };
            _contextMock.Setup(x => x.Usuarios).ReturnsDbSet(usuarios);

            // Act
            var result = await _userService.GetUserByEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result!.Email);
        }

        [Fact]
        public async Task GetUserByEmailAsync_NonExistingEmail_ReturnsNull()
        {
            // Arrange
            var email = "nonexistent@example.com";
            var usuarios = new List<Usuario>();
            _contextMock.Setup(x => x.Usuarios).ReturnsDbSet(usuarios);

            // Act
            var result = await _userService.GetUserByEmailAsync(email);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void VerifyPassword_CorrectPassword_ReturnsSuccess()
        {
            // Arrange
            var usuario = new Usuario { SenhaHash = "hashedpassword" };
            var password = "correctpassword";
            _passwordHasherMock
                .Setup(x => x.VerifyHashedPassword(usuario, usuario.SenhaHash, password))
                .Returns(PasswordVerificationResult.Success);

            // Act
            var result = _userService.VerifyPassword(usuario, password);

            // Assert
            Assert.Equal(PasswordVerificationResult.Success, result);
        }

        [Fact]
        public void VerifyPassword_IncorrectPassword_ReturnsFailed()
        {
            // Arrange
            var usuario = new Usuario { SenhaHash = "hashedpassword" };
            var password = "incorrectpassword";
            _passwordHasherMock
                .Setup(x => x.VerifyHashedPassword(usuario, usuario.SenhaHash, password))
                .Returns(PasswordVerificationResult.Failed);

            // Act
            var result = _userService.VerifyPassword(usuario, password);

            // Assert
            Assert.Equal(PasswordVerificationResult.Failed, result);
        }
    }
}

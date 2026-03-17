using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using FluentAssertions;
using ProyectoApi.Domain.Interfaces;
using ProyectoApi.Domain.Entities;
using ProyectoApi.Application.Services;
using Microsoft.Extensions.Logging;
using ProyectoApi.Application.Interfaces;
using ProyectoApi.Application.DTOs;
using Application.Tests.Builders;

namespace Application.Tests.Services
{
    public class UsuarioServiceTest
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ILogger<UsuarioService>> _loggerTestMock;
        private readonly UsuarioService _usuarioService;
        private readonly string _email = "test@gmail.com";
        private readonly string _password = "123456";
        public UsuarioServiceTest()
        {
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _loggerTestMock = new Mock<ILogger<UsuarioService>>();

            _usuarioService = new UsuarioService(_usuarioRepositoryMock.Object, _tokenServiceMock.Object, _passwordHasherMock.Object, _loggerTestMock.Object);
        }


        //___________________________TESTING LOGGIN___________________________
        //____________________________________________________________________


        [Fact]
        public async Task LoginAsync_UserNotFound_ReturnsFailure()
        {
            _usuarioRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(It.Is<string>(u => u.Equals(_email)))).ReturnsAsync((Usuario?)null);

            var request = new LoginDto
            {
                Email = _email,
                Password = _password
            };

            var result = await _usuarioService.LoginAsync(request);

            result.IsSuccess.Should().BeFalse();
            _usuarioRepositoryMock.Verify(repo => repo.GetByEmailAsync(It.Is<string>(u => u.Equals(_email))), Times.Once);
        }
        [Fact]
        public async Task LoginAsync_userInactive_ReturnsFailure()
        {
            var usuario = new UsuarioBuilder().Inactive().Build();

            _usuarioRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(It.Is<string>(u => u.Equals(_email)))).ReturnsAsync(usuario);

            var request = new LoginDto
            {
                Email = _email,
                Password = _password

            };

            var result = await _usuarioService.LoginAsync(request);

            result.IsSuccess.Should().BeFalse();
            _usuarioRepositoryMock.Verify(repo => repo.GetByEmailAsync(It.Is<string>(u => u.Equals(_email))), Times.Once);

        }

        [Fact]
        public async Task LoginAsync_InvalidPassWord_ReturnsFailure()
        {
            var usuario = new UsuarioBuilder().Build();

            _usuarioRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(usuario);

            _passwordHasherMock.Setup(hasher => hasher.Verify(_password, usuario.PassWordHash)).Returns(false);

            var request = new LoginDto
            {
                Email = _email,
                Password = _password
            };

            var result = await _usuarioService.LoginAsync(request);

            result.IsSuccess.Should().BeFalse();
            _usuarioRepositoryMock.Verify(repo => repo.GetByEmailAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {

            var usuario = new UsuarioBuilder().Build();

            _usuarioRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(_email)).ReturnsAsync(usuario);

            _passwordHasherMock.Setup(hasher => hasher.Verify(_password, usuario.PassWordHash)).Returns(true);

            _tokenServiceMock.Setup(service => service.GenerateToken(usuario)).Returns("token");

            var request = new LoginDto
            {
                Email = _email,
                Password = _password
            };

            var result = await _usuarioService.LoginAsync(request);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("token");

            _usuarioRepositoryMock.Verify(repo => repo.GetByEmailAsync(_email), Times.Once);
            _passwordHasherMock.Verify(hasher => hasher.Verify(_password, usuario.PassWordHash), Times.Once);
            _tokenServiceMock.Verify(service => service.GenerateToken(usuario), Times.Once);


        }

        //___________________________TESTING CREATE_USER___________________________
        //_________________________________________________________________________

        [Fact]
        public async Task CreateAsync_EmailAlreadyExists_ReturnsFailure()
        {
            _usuarioRepositoryMock
                .Setup(repo => repo.ExistEmail(It.Is<string>(u => u.Equals(_email)))).ReturnsAsync(true);
            var request = new CreateUsuarioDto
            {
                Nombre = "Test",
                Email = _email,
                Password = _password,
                RoleId = 1
            };

            var result = await _usuarioService.CreateAsync(request);
            result.Error.Should().Be("EmailDuplicado");
            _usuarioRepositoryMock.Verify(repo => repo.ExistEmail(It.Is<string>(u => u.Equals(_email))), Times.Once);

        }

        [Fact]
        public async Task CreateAsync_HashPassword_ReturnsSuccess()
        {
            _usuarioRepositoryMock
                .Setup(repo => repo.ExistEmail(It.IsAny<string>())).ReturnsAsync(false);
            _passwordHasherMock.Setup(hasher => hasher.Hash(It.Is<string>(u => u.Equals(_password)))).Returns("hashedPassword");
            var request = new CreateUsuarioDto
            {
                Nombre = "Test",
                Email = _email,
                Password = _password,
                RoleId = 1
            };

            var result = await _usuarioService.CreateAsync(request);
            result.IsSuccess.Should().BeTrue();

            _passwordHasherMock.Verify(hasher => hasher.Hash(It.Is<string>(u => u.Equals(_password))), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ValidUser_AddUserAndSaves()
        {
            _usuarioRepositoryMock
                .Setup(repo => repo.ExistEmail(_email)).ReturnsAsync(false);
            _passwordHasherMock.Setup(hash => hash.Hash(_password)).Returns("HashedPassword");

            var request = new CreateUsuarioDto
            {
                Nombre = "Test",
                Email = _email,
                Password = _password,
                RoleId = 1
            };

            var result = await _usuarioService.CreateAsync(request);
            result.IsSuccess.Should().BeTrue();

            _usuarioRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Usuario>(u =>
            u.Email == _email &&
            u.Nombre == "Test" &&
            u.PassWordHash == "HashedPassword" &&
            u.RolId == 1
            )), Times.Once);

            _usuarioRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        //___________________________TESTING DELETE USUARIO------------------------------
        //__-----------------------------------------------------------------------------

        [Fact]
        public async Task DeleteAsync_UserNotFound_ReturnsFailure()
        {

            var userId = 1;
            _usuarioRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((Usuario?)null);
            var result = await _usuarioService.DeleteAsync(userId);
            result.IsSuccess.Should().BeFalse();

            _usuarioRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _usuarioRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Usuario>()), Times.Never);
            _usuarioRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);

        }

        [Fact]
        public async Task DeleteUser_UserExists_RemovesAndSaves()
        {
            var userId = 1;
            var usuario = new UsuarioBuilder().WithId(userId).Build();
            _usuarioRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(usuario);

            var result = await _usuarioService.DeleteAsync(userId);

            result.IsSuccess.Should().BeTrue();

            _usuarioRepositoryMock.Verify(r => r.DeleteAsync(usuario), Times.Once);
            _usuarioRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        //___________________________TESTING UPDATE USUARIO____________________________
        //_____________________________________________________________________________

        [Fact]
        public async Task UpdateUser_UserNotFound_ReturnsFailure()
        {
            var userId = 1;

            _usuarioRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((Usuario?)null);
            var request = new UpdateUsuarioDto();
            var result = await _usuarioService.UpdateAsync(userId, request);
            result.IsSuccess.Should().BeFalse();

            _usuarioRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _usuarioRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Usuario>(), It.IsAny<byte[]>()), Times.Never);
            _usuarioRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);

        }

        [Fact]
        public async Task UpdateUser_EmailExist_ResultsFailure()
        {
            var userid = 1;
            var existEmail = _email;
            var usuario = new UsuarioBuilder().WithId(userid).Build();
            _usuarioRepositoryMock.Setup(r => r.GetByIdAsync(userid)).ReturnsAsync(usuario);
            var request = new UpdateUsuarioDto();
            request.Email = existEmail;

            _usuarioRepositoryMock.Setup(r => r.ExistEmailUpdate(request.Email, userid)).ReturnsAsync(true);

            var result = await _usuarioService.UpdateAsync(userid, request);
            result.IsSuccess.Should().BeFalse();

            _usuarioRepositoryMock.Verify(r => r.GetByIdAsync(userid), Times.Once);
            _usuarioRepositoryMock.Verify(r => r.ExistEmailUpdate(request.Email, userid), Times.Once);
            _usuarioRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Usuario>(), It.IsAny<byte[]>()), Times.Never);
            _usuarioRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);


        }
        [Fact]
        public async Task UpdateUser_ValidRequest_UpdatesAndSaves()
        {
            var userId = 1;
            var usuario = new UsuarioBuilder().WithId(userId).WithEmail("oldEmail@gmail.com").Build();

            _usuarioRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(usuario);
            _usuarioRepositoryMock.Setup(r => r.ExistEmailUpdate(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(false);

            var request = new UpdateUsuarioDto
            {
                Nombre = "New Name",
                Email = "New Email",
                RowVersion = [122]
            };

            var result = await _usuarioService.UpdateAsync(userId, request);

            result.IsSuccess.Should().BeTrue();

            _usuarioRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _usuarioRepositoryMock.Verify(r => r.ExistEmailUpdate(request.Email, userId), Times.Once);
            _usuarioRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Usuario>(u =>
            u.Nombre == request.Nombre &&
            u.Email == request.Email
            ), request.RowVersion), Times.Once);

            _usuarioRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}

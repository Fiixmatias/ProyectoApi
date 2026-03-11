using ProyectoApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests.Builders
{
    public class UsuarioBuilder
    {
        private readonly Usuario _usuario;

        public UsuarioBuilder()
        {
            _usuario = new Usuario
            {
                Id = 1,
                Nombre = "Test",
                Email = "test@gmail.com",
                Activo = true,
                PassWordHash = "hash",
                FechaCreacion = DateTime.Now,
                RolId = 1,
                RowVersion = new byte[12]
            };
        }

        public UsuarioBuilder WithEmail(string email)
        {
            _usuario.Email = email;
            return this;
        }

        public UsuarioBuilder Inactive()
        {
            _usuario.Activo = false;
            return this;
        }

        public UsuarioBuilder WithPasswordHash(string hash)
        {
            _usuario.PassWordHash = hash;
            return this;
        }
        public Usuario Build()
        {
            return _usuario;
        }
    }
}

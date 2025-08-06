
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Helpers;
using Sadvo.Core.Application.Interfaces.AdministradorInterfaces;
using Sadvo.Core.Domain.Entities.Administrador;
using SADVO.Core.Domain.Interfaces.AdminInterfaces;

namespace Sadvo.Core.Application.Services.AdministradorServices
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuariosRepository _repository;
        //private readonly IMapper _mapper;
        public UsuarioService(IUsuariosRepository repository)
        {
            _repository = repository;
            //_mapper = mapper;

        }
        public async Task<bool?> AddAsync(UsuarioDto usuario)
        {
            try
            {
                Usuario entity = new()
                {
                    Id = 0,
                    Nombre = usuario.Nombre ?? string.Empty,
                    Apellido = usuario.Apellido ?? string.Empty,
                    Email = usuario.Email ?? string.Empty,
                    NombreUsuario = usuario.NombreUsuario ?? string.Empty,
                    Contrasena = PasswoardEncryptation.EncryptPassword(usuario.Contrasena ?? string.Empty),
                    Rol = usuario.Rol,
                    Estado = usuario.Estado,
                };

                Usuario? entitydto = await _repository.AddAsync(entity);

                if (entitydto == null)
                {
                    return null;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al agregar usuario", ex);
            }
        }

        public async Task<bool?> UpdateAsync(int id, UsuarioDto usuario)
        {
            try
            {
                var entityExistente = await _repository.GetByIdAsync(id);

                if (entityExistente == null)
                {
                    return null;
                }

                Usuario entity = new()
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre ?? string.Empty,
                    Apellido = usuario.Apellido ?? string.Empty,
                    Email = usuario.Email ?? string.Empty,
                    NombreUsuario = usuario.NombreUsuario ?? string.Empty,
                    Contrasena = string.IsNullOrWhiteSpace(usuario.Contrasena) ? entityExistente.Contrasena : PasswoardEncryptation.EncryptPassword(usuario.Contrasena),
                    Rol = usuario.Rol,
                    Estado = usuario.Estado
                };

                Usuario? entitydto = await _repository.UpdateAsync(id, entity);
                if (entitydto == null)
                {
                    return null;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el usuario", ex);
            }
        }

        public async Task<List<Usuario>> GetAllListAsync()
        {
            try
            {
                var lista = await _repository.GetAllList();
                if (lista == null || !lista.Any())
                {
                    return new List<Usuario>();
                }

                return lista.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de puestos electivos", ex);
            }
        }


        public async Task<bool?> CambiarEstadoAsync(int id, UsuarioDto usuario)
        {
            try
            {

                var entity = await _repository.GetByIdAsync(id);

                if (entity == null)
                {
                    return false;
                }

                entity.Estado = usuario.Estado;

                Usuario? entitydto = await _repository.UpdateAsync(id, entity);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar el estado del puesto electivo", ex);
            }
        }

        public async Task<UsuarioDto> LogInAsync(LoginDto dto)
        {
            try
            {
                Usuario? usuario = await _repository.LoginAsync(dto.NombreUsuario, dto.contrasena);
                var usuarioCompleto = await _repository.GetByUsernameWithAsignacionAsync(dto.NombreUsuario);

                if (usuario == null)
                {
                    return null;
                }

                UsuarioDto usuariodto = new()
                {
                    Id = usuario.Id,
                    Email = usuario.Email,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Contrasena = usuario.Contrasena,
                    Estado = usuario.Estado,
                    NombreUsuario = usuario.NombreUsuario,
                    Rol = usuario.Rol,
                    PartidoPoliticoId = usuarioCompleto.AsignacionDirigente?.PartidoPoliticoId
                };


                return usuariodto;

            }
            catch (Exception ex)
            {
                throw new Exception("Error al ingresar en el sistema", ex);
            }
        }


        public async Task<bool?> CrearAdmin()
        {
            try
            {
                var verificacion = await _repository.HayAdmin();
                if (verificacion)
                {
                    return false;
                }

                Usuario user = new()
                {
                    Id = 0,
                    Nombre = "Admin",
                    Apellido = "n/a",
                    Email = "n/a",
                    NombreUsuario = "SuperAdmin",
                    Contrasena = PasswoardEncryptation.EncryptPassword("admin123"),
                    Rol = Domain.Enums.RolUsuario.Administrador,
                    Estado = Domain.Enums.Actividad.Activo
                };

                Usuario? usuarioEntity = await _repository.AddAsync(user);

                if (usuarioEntity == null)
                {
                    return null;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear administrador", ex);
            }
        }


        public async Task<UsuarioDto?> GetById(int id)
        {
            try
            {
                var resultado = await _repository.GetByIdAsync(id);
                if (resultado == null)
                { return null; }

                UsuarioDto usuario = new()
                {
                    Id = resultado.Id,
                    Nombre = resultado.Nombre,
                    Apellido = resultado.Apellido,
                    Email = resultado.Email,
                    NombreUsuario = resultado.NombreUsuario,
                    Contrasena = "",
                    Rol = resultado.Rol,
                    Estado = resultado.Estado
                };
                return usuario;

            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener usuario por id", ex);
            }
        }

        public async Task<ResultadoOperacion> ValidacionEleccionActiva()
        {
            if (await _repository.HayEleccionActiva())
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Accion denegada existe una elección activa"
                };

            }
            return new ResultadoOperacion
            {
                Exito = true,
                Mensaje = ""
            };
        }

        public async Task<ResultadoOperacion> ValidacionNombreExistente(UsuarioDto usuario)
        {
            if (await _repository.HayNombreExistenteAsync(usuario.NombreUsuario ?? string.Empty, usuario.Id))
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Error ya existe ese nombre de usuario"
                };
            }
            return new ResultadoOperacion
            {
                Exito = true,
                Mensaje = ""
            };
        }


        public async Task<Usuario?> GetByUsernameWithAsignacionAsync(string username)
        {
            return await _repository.GetByUsernameWithAsignacionAsync(username);
        }


    }
}

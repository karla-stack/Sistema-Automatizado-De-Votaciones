using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Helpers;
using Sadvo.Core.Application.Interfaces.AdministradorInterfaces;
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Interfaces.AdminInterfaces;

namespace Sadvo.Core.Application.Services.AdministradorServices
{
    public class AsignacionDirigenteService : IAsignacionDirigenteService
    {
        private readonly IAsignacionDirigenteRepository _repository;

        public AsignacionDirigenteService(IAsignacionDirigenteRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> AddAsync(int partidoPoliticoId, int usuarioId)
        {
            try
            {

                bool yaAsignado = await _repository.HayAsignacionExistente(usuarioId, partidoPoliticoId);
                if (yaAsignado)
                {
                    throw new Exception("Este dirigente ya está relacionado con otro partido político.");
                }

                var entity = new AsignacionDirigente
                {
                    PartidoPoliticoId = partidoPoliticoId,
                    UsuarioId = usuarioId
                };

                var entityResult = await _repository.AddAsync(entity);

                return entityResult != null;
            }
            catch (Exception ex)
            {
                // Puedes usar logging aquí si deseas
                throw new Exception("Error al asignar dirigente.", ex);
            }
        }

        public async Task<List<DirigentePoliticoDto>> GetListAsync()
        {
            try
            {
                var propiedades = new List<string> { "PartidoPolitico", "Usuario" };
                var lista = await _repository.GetAllListWithInclude(propiedades);

                var listaFiltrada = lista
                    .Where(a =>
                        a.PartidoPolitico != null && a.PartidoPolitico.Estado == Domain.Enums.Actividad.Activo &&
                        a.Usuario != null && a.Usuario.Estado == Domain.Enums.Actividad.Activo)
                    .Select(a => new DirigentePoliticoDto
                    {
                        Id = a.Id,
                        NombreUsuario = a.Usuario.NombreUsuario,
                        SiglasPartido = a.PartidoPolitico.Siglas
                    })
                    .ToList();

                return listaFiltrada;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de asignaciones de dirigentes", ex);
            }
        }


        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var asignacion = await _repository.GetByIdAsyncDirigente(id);
                if (asignacion == null)
                {
                    return false; // No se encontró la asignación
                }
                await _repository.DeleteAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la asignación de dirigente", ex);
            }
        }

        public async Task<List<UsuarioDto>> ObtenerUsuarios()
        {
            try
            {
                var lista = await _repository.ObtenerDirigentesDisponibles();

                if (lista == null)
                {
                    return new List<UsuarioDto>();
                }

                // Map the domain entities to DTOs
                var listaDto = lista.Select(usuario => new UsuarioDto
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Email = usuario.Email,
                    NombreUsuario = usuario.NombreUsuario,
                    Estado = usuario.Estado,
                    Rol = usuario.Rol
                }).ToList();

                return listaDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los usuarios", ex);
            }
        }



        public async Task<List<PartidoPoliticoDto>> ObtenerPartidoPolitico()
        {
            try
            {
                var lista = await _repository.ObtenerPartidosActivos();

                if (lista == null)
                {
                    return new List<PartidoPoliticoDto>();
                }

                // Map the domain entities to DTOs
                var listaDto = lista.Select(partido => new PartidoPoliticoDto
                {
                    Id = partido.Id,
                    Nombre = partido.Nombre,
                    Descripcion = partido.Descripcion,
                    Siglas = partido.Siglas,
                    Logo = partido.Logo,
                    Estado = partido.Estado
                }).ToList();

                return listaDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los partidos", ex);
            }
        }


        public async Task<AsignacionDirigenteDto?> GetById(int id)
        {
            try
            {
                var resultado = await _repository.GetByIdAsyncDirigente(id);
                if (resultado == null)
                { return null; }

                AsignacionDirigenteDto dirigente = new() { Id = resultado.Id, UsuarioId = resultado.UsuarioId, PartidoPoliticoId = resultado.PartidoPoliticoId, NombreUsuario = resultado.Usuario.NombreUsuario, SiglasPartido = resultado.PartidoPolitico.Siglas };
                return dirigente;

            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el dirigente por id", ex);
            }
        }


        public async Task<ResultadoOperacion> ValidacionEleccionActiva()
        {
            if (await _repository.HayEleccionActiva())
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Accion denegada existe una seleccion activa"
                };

            }
            return new ResultadoOperacion
            {
                Exito = true,
                Mensaje = ""
            };
        }


    }
}

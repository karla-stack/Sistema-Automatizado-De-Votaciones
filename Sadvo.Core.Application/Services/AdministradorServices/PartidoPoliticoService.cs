
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Helpers;
using Sadvo.Core.Application.Interfaces.AdministradorInterfaces;
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Interfaces.AdminInterfaces;

namespace Sadvo.Core.Application.Services.AdministradorServices
{
    public class PartidoPoliticoService : IPartidoPoliticoService
    {
        private readonly IPartidoPoliticoRepository _repository;

        public PartidoPoliticoService(IPartidoPoliticoRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool?> AddAsync(PartidoPoliticoDto partido)
        {
            try
            {
                PartidoPolitico entity = new()
                {
                    Id = 0,
                    Nombre = partido.Nombre ?? string.Empty,
                    Descripcion = partido.Descripcion,
                    Siglas = partido.Siglas ?? string.Empty,
                    Estado = partido.Estado,
                    Logo = partido.Logo
                };

                PartidoPolitico? entitydto = await _repository.AddAsync(entity);

                if (entitydto == null)
                {
                    return null;
                }

                return true;

            }
            catch (Exception ex)
            {
                throw new NotImplementedException("Error al agregar un partido politico", ex);
            }
        }

        public async Task<bool?> UpdateAsync(int id, PartidoPoliticoDto partido)
        {
            try
            {

                var entityExistente = await _repository.GetByIdAsync(id);

                if (entityExistente == null)
                {
                    return false;
                }

                PartidoPolitico entity = new()
                {
                    Id = partido.Id,
                    Nombre = partido.Nombre ?? string.Empty,
                    Descripcion = partido.Descripcion,
                    Siglas = partido.Siglas ?? string.Empty,
                    Estado = partido.Estado,
                    Logo = partido.Logo
                };

                PartidoPolitico? entitydto = await _repository.UpdateAsync(id, entity);
                if (entitydto == null)
                {
                    return null;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el partido politico", ex);
            }
        }
        public async Task<bool> CambiarEstadoAsync(int id, PartidoPoliticoDto partido)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);

                if (entity == null)
                {
                    return false;
                }

                entity.Estado = partido.Estado;

                PartidoPolitico? entitydto = await _repository.UpdateAsync(id, entity);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar el estado del partido politico", ex);
            }
        }

        public async Task<List<PartidoPolitico>> GetAllListAsync()
        {
            try
            {
                var lista = await _repository.GetAllList();
                if (lista == null || !lista.Any())
                {
                    return new List<PartidoPolitico>();
                }

                return lista.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de partidos politicos", ex);
            }
        }


        public async Task<PartidoPoliticoDto?> GetById(int id)
        {
            try
            {
                var resultado = await _repository.GetByIdAsync(id);
                if (resultado == null)
                { return null; }

                PartidoPoliticoDto partido = new()
                {
                    Id = resultado.Id,
                    Nombre = resultado.Nombre,
                    Descripcion = resultado.Descripcion,
                    Siglas = resultado.Siglas,
                    Estado = resultado.Estado,
                    Logo = resultado.Logo
                };
                return partido;

            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener partido por id", ex);
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
    }
}

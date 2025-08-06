using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Helpers;
using Sadvo.Core.Application.Interfaces.AdministradorInterfaces;
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Interfaces.AdminInterfaces;

namespace Sadvo.Core.Application.Services.AdministradorServices
{
    public class CiudadanoService : ICiudadanoService
    {
        private readonly ICiudadanoRepository _repository;

        public CiudadanoService(ICiudadanoRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool?> AddAsync(CiudadanoDto ciudadano)
        {
            try
            {

                Ciudadano entity = new()
                {
                    Id = 0,
                    Nombre = ciudadano.Nombre ?? string.Empty,
                    Apellido = ciudadano.Apellido ?? string.Empty,
                    Email = ciudadano.Email ?? string.Empty,
                    Estado = Domain.Enums.Actividad.Activo,
                    Identificacion = ciudadano.Identificacion ?? string.Empty
                };

                Console.WriteLine($"Llamando a _repository.AddAsync...");
                Ciudadano? addedCiudadano = await _repository.AddAsync(entity);

                if (addedCiudadano != null)
                {
                    Console.WriteLine($"Ciudadano agregado exitosamente");
                }
                else
                {
                    Console.WriteLine($"ERROR: addedCiudadano es NULL");
                }

                Console.WriteLine($"=== SERVICE AddAsync COMPLETADO ===");
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al agregar ciudadano", ex);
            }
        }


        public async Task<bool?> UpdateAsync(int id, CiudadanoDto ciudadano)
        {
            try
            {
                var ciudadanoExistente = await _repository.GetByIdAsync(id);
                if (ciudadanoExistente == null) return null;

                // ✅ CORRECTO: Actualizar la entidad existente
                ciudadanoExistente.Nombre = ciudadano.Nombre ?? string.Empty;
                ciudadanoExistente.Apellido = ciudadano.Apellido ?? string.Empty;
                ciudadanoExistente.Email = ciudadano.Email ?? string.Empty;
                ciudadanoExistente.Identificacion = ciudadano.Identificacion ?? string.Empty;
                ciudadanoExistente.Estado = ciudadano.Estado;

                await _repository.UpdateAsync(id, ciudadanoExistente);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el ciudadano", ex);
            }
        }


        public async Task<bool> CambiarEstadoAsync(int id, CiudadanoDto puesto)
        {
            try
            {

                var entity = await _repository.GetByIdAsync(id);

                if (entity == null)
                {
                    return false;
                }

                entity.Estado = puesto.Estado;

                Ciudadano? entitydto = await _repository.UpdateAsync(id, entity);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar el estado del ciudadano", ex);
            }
        }

        public async Task<List<Ciudadano>> GetAllListAsync()
        {
            try
            {
                var lista = await _repository.GetAllList();
                if (lista == null || !lista.Any())
                {
                    return new List<Ciudadano>();
                }

                return lista.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de puestos electivos", ex);
            }
        }



        public async Task<CiudadanoDto?> GetById(int id)
        {
            try
            {
                var resultado = await _repository.GetByIdAsync(id);
                if (resultado == null)
                { return null; }

                CiudadanoDto ciudadano = new() { Id = resultado.Id, Nombre = resultado.Nombre, Apellido = resultado.Apellido, Email = resultado.Email, Identificacion = resultado.Identificacion, Estado = resultado.Estado };
                return ciudadano;

            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener ciudadano por id", ex);
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


        public async Task<ResultadoOperacion> ValidacionCedulaExistente(CiudadanoDto ciudadano)
        {
            if (await _repository.HayCedulaExistente(ciudadano.Identificacion ?? string.Empty, ciudadano.Id))
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Error ya existe esa cedula"
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

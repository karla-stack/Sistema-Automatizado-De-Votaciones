using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Helpers;
using Sadvo.Core.Application.Interfaces.AdministradorInterfaces;
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Interfaces.AdminInterfaces;

namespace Sadvo.Core.Application.Services.AdministradorServices
{
    public class PuestoElectivoService : IPuestoElectivoService
    {

        private readonly IPuestoElectivoRepository _repository;

        public PuestoElectivoService(IPuestoElectivoRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool?> AddAsync(PuestoElectivoDto puesto)
        {
            try
            {

                PuestoElectivo entity = new()
                {
                    Id = 0,
                    Nombre = puesto.Nombre ?? string.Empty,
                    Descripcion = puesto.Descripcion ?? string.Empty,
                    Estado = Domain.Enums.Actividad.Activo
                };

                PuestoElectivo? entitydto = await _repository.AddAsync(entity);

                if (entitydto == null)
                {
                    return null;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al agregar el puesto electivo", ex);
            }
        }

        public async Task<bool?> UpdateAsync(int id, PuestoElectivoDto puesto)
        {
            try
            {
                var entityExistente = await _repository.GetByIdAsync(id);
                if (entityExistente == null) return null;

                entityExistente.Nombre = puesto.Nombre ?? string.Empty;
                entityExistente.Descripcion = puesto.Descripcion ?? string.Empty;
                entityExistente.Estado = puesto.Estado;

                await _repository.UpdateAsync(id, entityExistente);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el puesto electivo", ex);
            }
        }

        public async Task<List<PuestoElectivo>> GetAllListAsync()
        {
            try
            {
                var lista = await _repository.GetAllList();
                if (lista == null || !lista.Any())
                {
                    return new List<PuestoElectivo>();
                }

                return lista.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de puestos electivos", ex);
            }
        }


        public async Task<bool?> CambiarEstadoAsync(int id, PuestoElectivoDto puesto)
        {
            try
            {

                var entity = await _repository.GetByIdAsync(id);

                if (entity == null)
                {
                    return false;
                }

                entity.Estado = puesto.Estado;

                PuestoElectivo? entitydto = await _repository.UpdateAsync(id, entity);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar el estado del puesto electivo", ex);
            }
        }

        public async Task<PuestoElectivoDto?> GetById(int id)
        {
            try
            {
                var resultado = await _repository.GetByIdAsync(id);
                if (resultado == null)
                { return null; }

                PuestoElectivoDto puesto = new() { Id = resultado.Id, Nombre = resultado.Nombre, Descripcion = resultado.Descripcion, Estado = resultado.Estado };
                return puesto;

            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener puesto electivo por id", ex);
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

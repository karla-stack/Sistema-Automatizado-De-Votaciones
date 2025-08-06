// Interfaces/AdministradorInterfaces/IEleccionService.cs
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Helpers;

namespace Sadvo.Core.Application.Interfaces.AdministradorInterfaces
{
    public interface IEleccionService
    {
        // Listado y consultas
        Task<List<EleccionResumenDto>> GetListAsync();
        Task<List<EleccionDetalleDto>> GetByIdResult(int id);
        Task<EleccionDto?> GetById(int id);

        // Elección activa
        Task<bool> HayEleccionActiva();
        Task<EleccionDto?> GetEleccionActiva();
        Task<bool> FinalizarEleccion(int eleccionId);

        // Crear nueva elección
        Task<ResultadoValidacionEleccion> ValidarCreacionEleccion();
        Task<bool> AddAsync(EleccionDto entity);
    }
}
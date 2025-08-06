
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Helpers;
using Sadvo.Core.Domain.Entities.Administrador;

namespace Sadvo.Core.Application.Interfaces.AdministradorInterfaces
{
    public interface IPuestoElectivoService
    {
        Task<bool?> AddAsync(PuestoElectivoDto puesto);
        Task<List<PuestoElectivo>> GetAllListAsync();
        Task<bool?> UpdateAsync(int id, PuestoElectivoDto puesto);
        Task<bool?> CambiarEstadoAsync(int id, PuestoElectivoDto puesto);
        Task<ResultadoOperacion> ValidacionEleccionActiva();
        Task<PuestoElectivoDto?> GetById(int id);
    }
}

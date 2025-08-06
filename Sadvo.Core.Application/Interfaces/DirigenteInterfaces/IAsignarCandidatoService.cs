
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Helpers;

namespace Sadvo.Core.Application.Interfaces.DirigenteInterfaces
{
    public interface IAsignarCandidatoService
    {
        Task<bool> AddAsync(int puestoElectivoId, int candidatoId);

        Task<ResultadoOperacion> ValidacionEleccionActiva();

        Task<bool> DeleteAsync(int id);

        Task<List<PuestoCandidatoDto>> GetListAsync();

        Task<List<CandidatoDto>> ObtenerCandidato();

        Task<List<PuestoElectivoDto>> ObtenerPuestoElectivo();

        Task<AsignacionCandidatoDto?> GetById(int id);
    }
}

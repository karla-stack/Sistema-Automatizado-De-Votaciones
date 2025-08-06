
using System.Threading.Tasks;
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Entities.Dirigente;

namespace Sadvo.Core.Domain.Interfaces.DirigenteInterfaces
{
    public interface IAsignacionCandidatoRepository : IGenericRepository<AsignacionCandidatoPuesto>
    {
        Task<bool> HayEleccionActiva();
        Task<List<AsignacionCandidatoPuesto>> ListaAsignacionCandidatoPuesto(int partidoDirigenteId);
        Task<List<Candidato>> ObtenerCandidatosDisponibles(int partidoDirigenteId);
        Task<List<PuestoElectivo>> ObtenerPuestosDisponibles(int partidoDirigenteId);
        Task<bool> ValidarCandidatoParaPuesto(int candidatoId, int puestoElectivoId, int partidoDirigenteId);
        Task<List<PuestoElectivo>> ObtenerPuestosDisponiblesParaCandidato(int candidatoId, int partidoDirigenteId);
    }
}

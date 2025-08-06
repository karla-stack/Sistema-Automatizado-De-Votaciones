
using Sadvo.Core.Domain.Entities.Dirigente;

namespace Sadvo.Core.Domain.Interfaces.DirigenteInterfaces
{
    public interface ICandidatoRepository : IGenericRepository<Candidato>
    {
        Task<bool> HayEleccionActiva();
        Task<bool> EstaAsociadoPuesto(int candidatoId);
        Task<List<Candidato>> GetListCandidatos();

        Task<Candidato?> GetByIdAsyncPartido(int id);

        Task<List<AsignacionCandidatoPuesto>> ListaAsignacionCandidatoPuesto(int partidoDirigenteId);

        Task<List<Candidato>> GetCandidatosByPartido(int partidoId);

        Task<string?> ObtenerPuestoAsignadoPorPartido(int candidatoId, int partidoId);
    }
}

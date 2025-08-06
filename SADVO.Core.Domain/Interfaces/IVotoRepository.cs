using Sadvo.Core.Domain.Entities.Dirigente;
using Sadvo.Core.Domain.Entities.Elector;

namespace Sadvo.Core.Domain.Interfaces
{
    public interface IVotoRepository
    {
        Task<bool> HasVotedForPuestoAsync(int ciudadanoId, int puestoId);
        Task<bool> HasVotedInActiveElectionAsync(int ciudadanoId);
        Task<List<Voto>> GetVotedPuestosByElectorAsync(int ciudadanoId);
        Task<Voto> CreateVoteAsync(Voto voto);
        Task<int> GetVotedPuestosCountAsync(int ciudadanoId);
        Task<List<Candidato>> GetCandidatosByPuestoAsync(int puestoId);
    }
}

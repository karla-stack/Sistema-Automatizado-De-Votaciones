

using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Helpers;
using Sadvo.Core.Domain.Entities.Dirigente;

namespace Sadvo.Core.Application.Interfaces.DirigenteInterfaces
{
    public interface ICandidatoService 
    {
        Task<bool?> AddAsync(CandidatoDto candidato);

        Task<bool?> UpdateAsync(int id, CandidatoDto candidato);

        Task<List<Candidato>> GetAllListAsync();

        Task<bool?> CambiarEstadoAsync(int id, CandidatoDto candidato);

        Task<CandidatoDto?> GetById(int id);

        Task<ResultadoOperacion> ValidacionEleccionActiva();

        Task<List<PuestoCandidatoDto>> GetListCandidato();

        Task<List<CandidatoDto>> GetListCandidatosByPartido(int partidoId);

    }
}

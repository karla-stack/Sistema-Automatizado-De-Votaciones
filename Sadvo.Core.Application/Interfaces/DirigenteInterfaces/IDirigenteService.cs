
using Sadvo.Core.Application.Dtos;

namespace Sadvo.Core.Application.Interfaces.DirigenteInterfaces
{
    public interface IDirigenteService
    {
        Task<DirigenteHomeDto> GetDashboardAsync(int usuarioId);

        Task<PartidoPoliticoHomeDto> GetPartidoInfoAsync(int usuarioId);

        Task<IndicadoresDirigenteDto> GetIndicadoresAsync(int usuarioId);
    }
}

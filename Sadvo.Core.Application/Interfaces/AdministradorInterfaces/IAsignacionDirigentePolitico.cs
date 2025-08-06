
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Helpers;

namespace Sadvo.Core.Application.Interfaces.AdministradorInterfaces
{
    public interface IAsignacionDirigenteService
    {
        Task<bool> AddAsync(int partidoPoliticoId, int usuarioId);
        Task<bool> DeleteAsync(int id);
        Task<List<DirigentePoliticoDto>> GetListAsync();
        Task<List<PartidoPoliticoDto>> ObtenerPartidoPolitico();
        Task<List<UsuarioDto>> ObtenerUsuarios();
        Task<AsignacionDirigenteDto?> GetById(int id);

        Task<ResultadoOperacion> ValidacionEleccionActiva();
    }
}


using Sadvo.Core.Domain.Entities.Administrador;

namespace Sadvo.Core.Domain.Interfaces.AdminInterfaces
{
    public interface IAsignacionDirigenteRepository : IGenericRepository<AsignacionDirigente>
    {
        Task<bool> HayEleccionActiva();
        Task<bool> HayAsignacionExistente(int usuarioId, int partidoId);
        Task<List<PartidoPolitico>> ObtenerPartidosActivos();
        Task<List<Usuario>> ObtenerDirigentesDisponibles();

        Task<List<AsignacionDirigente>> ListaAsignacionDirigentesActivos();

        Task<AsignacionDirigente?> GetByIdAsyncDirigente(int id);
    }
}

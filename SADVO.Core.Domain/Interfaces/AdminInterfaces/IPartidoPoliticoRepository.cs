
using Sadvo.Core.Domain.Entities.Administrador;

namespace Sadvo.Core.Domain.Interfaces.AdminInterfaces
{
    public interface IPartidoPoliticoRepository : IGenericRepository<PartidoPolitico>
    {
        Task<bool> HayEleccionActiva();
    }
}

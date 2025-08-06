
using Sadvo.Core.Domain.Entities.Administrador;

namespace Sadvo.Core.Domain.Interfaces.AdminInterfaces
{
    public interface IPuestoElectivoRepository : IGenericRepository<PuestoElectivo>
    {
        Task<bool> HayEleccionActiva();
    }
}

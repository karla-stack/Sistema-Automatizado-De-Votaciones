
using Sadvo.Core.Domain.Entities.Administrador;

namespace Sadvo.Core.Domain.Interfaces.AdminInterfaces
{
    public interface ICiudadanoRepository : IGenericRepository<Ciudadano>
    {
        Task<bool> HayEleccionActiva();

        Task<bool> HayCedulaExistente(string Identificacion);

        Task<bool> HayCedulaExistente(string Identificacion, int idIgnorar);
    }
}

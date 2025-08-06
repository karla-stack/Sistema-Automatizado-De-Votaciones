
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Interfaces;

namespace SADVO.Core.Domain.Interfaces.AdminInterfaces
{
    public interface IUsuariosRepository : IGenericRepository<Usuario>
    {
        Task<bool> HayEleccionActiva();
        Task<bool> HayNombreExistenteAsync(string nombreUsuario);

        Task<bool> HayNombreExistenteAsync(string nombreUsuario, int idIgnorar);

        Task<Usuario?> LoginAsync(string username, string password);

        Task<bool> HayAdmin();

        Task<Usuario?> GetByUsernameWithAsignacionAsync(string username);
    }
}


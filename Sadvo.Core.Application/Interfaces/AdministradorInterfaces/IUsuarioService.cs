
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Helpers;
using Sadvo.Core.Domain.Entities.Administrador;

namespace Sadvo.Core.Application.Interfaces.AdministradorInterfaces
{
    public interface IUsuarioService
    {
        Task<bool?> AddAsync(UsuarioDto usuario);
        Task<List<Usuario>> GetAllListAsync();
        Task<bool?> UpdateAsync(int id, UsuarioDto usuario);
        Task<bool?> CambiarEstadoAsync(int id, UsuarioDto usuario);
        Task<UsuarioDto?> GetById(int id);
        Task<UsuarioDto> LogInAsync(LoginDto dto);
        Task<bool?> CrearAdmin();
        Task<ResultadoOperacion> ValidacionNombreExistente(UsuarioDto usuario);
        Task<ResultadoOperacion> ValidacionEleccionActiva();

        Task<Usuario?> GetByUsernameWithAsignacionAsync(string username);
    }
}

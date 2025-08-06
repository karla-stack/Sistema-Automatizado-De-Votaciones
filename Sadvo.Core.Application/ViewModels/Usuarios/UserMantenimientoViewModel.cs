
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Application.ViewModels.Usuarios
{
    public class UserMantenimientoViewModel
    {

        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Email { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Contrasena { get; set; }
        public Actividad Estado { get; set; }
        public RolUsuario Rol { get; set; }

        public string? Mensaje { get; set; }
    }
}

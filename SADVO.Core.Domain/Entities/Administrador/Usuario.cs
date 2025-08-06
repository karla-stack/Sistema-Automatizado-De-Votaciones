
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Domain.Entities.Administrador
{
    public class Usuario
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public required string Email { get; set; }
        public required string NombreUsuario { get; set; }
        public required string Contrasena { get; set; }
        public Actividad Estado { get; set; }
        public required RolUsuario Rol { get; set; }
        public AsignacionDirigente? AsignacionDirigente { get; set; }
    }
}

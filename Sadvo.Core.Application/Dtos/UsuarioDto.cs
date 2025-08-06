
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Application.Dtos
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Email { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Contrasena { get; set; }
        public required Actividad Estado { get; set; }
        public RolUsuario Rol { get; set; }

        public int? PartidoPoliticoId { get; set; }
    }
}

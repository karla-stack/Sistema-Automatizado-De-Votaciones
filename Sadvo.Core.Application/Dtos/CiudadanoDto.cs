
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Application.Dtos
{
    public class CiudadanoDto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Email { get; set; }
        public Actividad Estado { get; set; }
        public string? Identificacion { get; set; }
    }
}

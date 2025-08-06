
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Application.Dtos
{
    public class PartidoPoliticoDto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? Siglas { get; set; }
        public string? Logo { get; set; }
        public required Actividad Estado { get; set; }
    }
}


using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Application.Dtos
{
    public class CandidatoConPuestoDto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Foto { get; set; }
        public Actividad Estado { get; set; }
        public int PartidoPoliticoId { get; set; }
        public string PuestoAsociado { get; set; } = "Sin puesto asociado";
    }
}


using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Application.Dtos
{
    public class PuestoElectivoDto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public Actividad Estado { get; set; }
        public string? Descripcion { get; set; }
    }
}

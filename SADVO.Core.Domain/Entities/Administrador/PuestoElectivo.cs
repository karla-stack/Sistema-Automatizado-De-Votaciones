
using Sadvo.Core.Domain.Entities.Dirigente;
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Domain.Entities.Administrador
{
    public class PuestoElectivo
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public Actividad Estado { get; set; }
        public required string Descripcion { get; set; }
        public ICollection<AsignacionCandidatoPuesto>? CandidatosAsignados { get; set; }
    }
}

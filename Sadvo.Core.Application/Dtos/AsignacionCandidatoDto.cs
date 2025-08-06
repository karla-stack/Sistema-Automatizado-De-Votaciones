

namespace Sadvo.Core.Application.Dtos
{
    public class AsignacionCandidatoDto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? PuestoAsignado { get; set; }
        public int CandidatoId { get; set; }
        public int PartidoPoliticoId { get; set; }
        public int PuestoElectivoId { get; set; }
    }
}

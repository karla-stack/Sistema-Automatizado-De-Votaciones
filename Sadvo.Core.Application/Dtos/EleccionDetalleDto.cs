

namespace Sadvo.Core.Application.Dtos
{
    public class EleccionDetalleDto
    {
        public int PuestoElectivoId { get; set; }
        public string NombrePuesto { get; set; } = "";
        public List<CandidatoResultadoDto> Candidatos { get; set; } = new();
    }
}

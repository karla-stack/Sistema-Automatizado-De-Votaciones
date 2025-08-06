

namespace Sadvo.Core.Application.ViewModels.Eleccion
{
    public class EleccionDetalleViewModel
    {
        public int PuestoElectivoId { get; set; }
        public string NombrePuesto { get; set; } = "";
        public List<CandidatoResultadoViewModel> Candidatos { get; set; } = new();
    }
}

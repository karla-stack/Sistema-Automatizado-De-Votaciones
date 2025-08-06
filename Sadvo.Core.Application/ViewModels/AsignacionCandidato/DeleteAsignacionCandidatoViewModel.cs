

namespace Sadvo.Core.Application.ViewModels.AsignacionCandidato
{
    public class DeleteAsignacionCandidatoViewModel
    {
        public int Id { get; set; }
        public int PuestoElectivoId { get; set; }
        public int CandidatoId { get; set; }
        public string? NombreCandidato { get; set; }
        public string? NombrePuestoElectivo { get; set; }  // para mostrar en vista
        public string? Mensaje { get; set; }
    }
}

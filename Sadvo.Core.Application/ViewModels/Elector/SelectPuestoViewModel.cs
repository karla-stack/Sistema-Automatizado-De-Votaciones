

namespace Sadvo.Core.Application.ViewModels.Elector
{
    public class SelectPuestoViewModel
    {
        public int PuestoId { get; set; }
        public string PuestoNombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int TotalCandidatos { get; set; }
        public bool HasVoted { get; set; }
        public bool IsActive { get; set; }
        public string StatusText => HasVoted ? "✓ Votado" : "Pendiente";
        public string StatusClass => HasVoted ? "text-success" : "text-warning";
    }
}

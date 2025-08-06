

using System.ComponentModel.DataAnnotations;

namespace Sadvo.Core.Application.ViewModels.Eleccion
{
    public class EleccionListModeloViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public DateOnly Fecha { get; set; }
        public int CantPuestos { get; set; }
        public int CantPartidos { get; set; }
        public bool EstaFinalizada { get; set; }
        public bool EstaActiva { get; set; }
        public string FechaFormateada => Fecha.ToString("dd/MM/yyyy");
        public string EstadoTexto => EstaActiva ? "Activa" : (EstaFinalizada ? "Finalizada" : "En proceso");
        public string EstadoBadgeClass => EstaActiva ? "badge-success" : (EstaFinalizada ? "badge-secondary" : "badge-warning");
    }
}

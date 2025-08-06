

namespace Sadvo.Core.Application.ViewModels.Eleccion
{
    public class ConfirmarFinalizarEleccionViewModel
    {
        public int EleccionId { get; set; }
        public string NombreEleccion { get; set; } = "";
        public DateOnly FechaEleccion { get; set; }
        public string FechaFormateada => FechaEleccion.ToString("dd/MM/yyyy");
    }
}

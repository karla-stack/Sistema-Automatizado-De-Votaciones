

namespace Sadvo.Core.Application.ViewModels.Eleccion
{
    public class EleccionResultadosViewModel
    {
        public int EleccionId { get; set; }
        public string NombreEleccion { get; set; } = "";
        public DateOnly FechaEleccion { get; set; }
        public List<EleccionDetalleViewModel> Resultados { get; set; } = new();
    }
}



namespace Sadvo.Core.Application.ViewModels.Eleccion
{
    public class EleccionesListViewModel
    {
        public List<EleccionListModeloViewModel> elecciones { get; set; } = new();
        public bool HayEleccionActiva { get; set; }
    }
}

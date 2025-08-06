using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadvo.Core.Application.ViewModels.Administrador
{
    public class AdministradorMenuViewModel
    {
        public List<int> AñosDisponibles { get; set; } = new();
        public int AñoSeleccionado { get; set; }
        public List<ResumenElectoralViewModel> ResumenElectoral { get; set; } = new();
        public string UsuarioNombre { get; set; } = string.Empty;
        public bool MostrarResumen { get; set; }
        public bool TieneError { get; set; }
        public string MensajeError { get; set; } = string.Empty;

        // Propiedades para la vista
        public bool TieneAñosDisponibles => AñosDisponibles.Any();
        public bool TieneElecciones => ResumenElectoral.Any();
        public int TotalElecciones => ResumenElectoral.Count;
        public int TotalVotosGeneral => ResumenElectoral.Sum(r => r.TotalVotos);
        public int TotalPartidosUnicos => ResumenElectoral.SelectMany(r => Enumerable.Range(1, r.CantidadPartidos)).Distinct().Count();

        // Texto para mostrar el año seleccionado
        public string AñoSeleccionadoTexto => AñoSeleccionado.ToString();

        // Mensaje de estado del resumen
        public string MensajeResumen => MostrarResumen
            ? (TieneElecciones
                ? $"Se encontraron {TotalElecciones} elección(es) en el año {AñoSeleccionado}"
                : $"No se encontraron elecciones en el año {AñoSeleccionado}")
            : "Seleccione un año para ver el resumen electoral";
    }
}

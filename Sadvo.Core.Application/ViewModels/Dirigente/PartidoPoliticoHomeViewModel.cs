using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadvo.Core.Application.ViewModels.Dirigente
{
    public class PartidoPoliticoHomeViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Siglas { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? Descripcion { get; set; }
        public bool EstaActivo { get; set; }

        // Propiedades para la vista
        public bool TieneLogo => !string.IsNullOrEmpty(LogoUrl);
        public string LogoUrlSegura => TieneLogo ? LogoUrl! : "/images/default-party-logo.png";
        public string EstadoTexto => EstaActivo ? "Activo" : "Inactivo";
        public string EstadoClase => EstaActivo ? "text-success" : "text-danger";
        public string EstadoIcono => EstaActivo ? "fas fa-check-circle" : "fas fa-times-circle";
    }
}

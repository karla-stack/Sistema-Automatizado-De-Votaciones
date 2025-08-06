using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sadvo.Core.Application.ViewModels.PartidoPolitico;

namespace Sadvo.Core.Application.ViewModels.Dirigente
{
    public class DirigenteMenuViewModel
    {
        public PartidoPoliticoHomeViewModel PartidoInfo { get; set; } = new();
        public IndicadoresDirigenteViewModel Indicadores { get; set; } = new();
        public bool TieneEleccionActiva { get; set; }
        public string EleccionActivaNombre { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
        public bool TieneError { get; set; }
        public string MensajeError { get; set; } = string.Empty;
    }
}

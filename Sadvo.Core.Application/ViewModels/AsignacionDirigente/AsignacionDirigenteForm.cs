using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sadvo.Core.Application.Dtos;

namespace Sadvo.Core.Application.ViewModels.AsignacionDirigente
{
    public class AsignacionDirigenteFormViewModel
    {

        public int IdPartido { get; set; }
        public int IdUsuario { get; set; }

        public List<PartidoPoliticoDto>? Partidos { get; set; }
        public List<UsuarioDto>? Usuarios { get; set; }

        public string? Mensaje { get; set; }
    }
}

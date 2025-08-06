using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadvo.Core.Application.Dtos
{
    public class PartidoPoliticoHomeDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Siglas { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? Descripcion { get; set; }
        public bool EstaActivo { get; set; }
    }
}

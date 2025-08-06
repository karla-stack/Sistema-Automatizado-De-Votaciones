
using System.ComponentModel.DataAnnotations;

namespace Sadvo.Core.Application.ViewModels.AsignacionDirigente
{
    public class AsignacionDirigenteViewModel
    {
        public int Id { get; set; }
        [Required]
        public string? NombreUsuario { get; set; }

        [Required]
        public string? SiglasPartido { get; set; }
    }
}

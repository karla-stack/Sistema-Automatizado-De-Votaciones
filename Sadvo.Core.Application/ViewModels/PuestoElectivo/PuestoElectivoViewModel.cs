
using System.ComponentModel.DataAnnotations;
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Application.ViewModels.PuestoElectivo
{
    public class PuestoElectivoViewModel
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string? Nombre { get; set; }
        public Actividad Estado { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string? Descripcion { get; set; }

        public string? Mensaje { get; set; }
    }
}

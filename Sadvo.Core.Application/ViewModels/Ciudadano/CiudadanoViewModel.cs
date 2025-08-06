
using System.ComponentModel.DataAnnotations;
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Application.ViewModels.Ciudadano
{
    public class CiudadanoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; } = null!;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ser un correo válido")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "La identificación es obligatoria")]
        public string Identificacion { get; set; } = null!;

        public Actividad Estado { get; set; }
        public string? Mensaje { get; set; }
    }
}

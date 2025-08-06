

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Application.ViewModels.PartidoPolitico
{
    public class PartidoPoliticoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del partido es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string? Nombre { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "Las siglas son requeridas")]
        [StringLength(10, ErrorMessage = "Las siglas no pueden exceder 10 caracteres")]
        public string? Siglas { get; set; }

        public Actividad Estado { get; set; }

        // Para subir nuevo logo
        [DataType(DataType.Upload)]
        public IFormFile? Logo { get; set; }

        // Para mostrar el logo actual (ruta de la base de datos)
        public string? LogoPath { get; set; }

        public string? Mensaje { get; set; }
    }
}

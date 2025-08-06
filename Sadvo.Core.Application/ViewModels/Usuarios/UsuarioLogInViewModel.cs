
using System.ComponentModel.DataAnnotations;

namespace Sadvo.Core.Application.ViewModels.Usuarios
{
    public class UsuarioLogInViewModel
    {
        [Required(ErrorMessage = "Introduce el usuario")]
        [DataType(DataType.Text)]
        public string? NombreUsuario { get; set; }

        [Required(ErrorMessage = "Introduce la contraseña")]
        [DataType(DataType.Password)]
        public string? Contrasena { get; set; }

        public string? Mensaje { get; set; }
    }
}

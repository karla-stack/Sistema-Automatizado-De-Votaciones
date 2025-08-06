
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Application.ViewModels
{
    public class ConfirmarEstadoViewModel
    {
        public int UsuarioId { get; set; }
        public string? NombreUsuario { get; set; }
        public Actividad EstadoActual { get; set; }
        public Actividad NuevoEstado { get; set; }

        public string? Mensaje { get; set; }

        public string MensajeConfirmacion
        {
            get
            {
                return NuevoEstado == Actividad.Activo
                    ? "¿Está seguro que desea activar este usuario?"
                    : "¿Está seguro que desea desactivar este usuario?";
            }
        }

        public string AccionTexto
        {
            get
            {
                return NuevoEstado == Actividad.Activo ? "Activar" : "Desactivar";
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sadvo.Core.Domain.Entities.Administrador;

namespace Sadvo.Core.Domain.Interfaces.AdminInterfaces
{
    public interface IAdministradorHomeRepository
    {
        // Obtener años con elecciones
        Task<List<int>> GetAñosConEleccionesAsync();

        // Obtener elecciones por año
        Task<List<Eleccion>> GetEleccionesPorAñoAsync(int año);

        // Estadísticas de elecciones
        Task<int> GetCantidadPartidosParticipantesAsync(int eleccionId);
        Task<int> GetCantidadCandidatosParticipantesAsync(int eleccionId);
        Task<int> GetCantidadTotalVotosAsync(int eleccionId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Entities.Dirigente;

namespace Sadvo.Core.Domain.Interfaces.DirigenteInterfaces
{
    public interface IDirigenteRepository 
    {

        Task<PartidoPolitico?> GetPartidoPoliticoByDirigenteAsync(int usuarioId);

        // Candidatos
        Task<List<Candidato>> GetCandidatosByPartidoAsync(int partidoId);
        Task<int> GetCantidadCandidatosActivosAsync(int partidoId);
        Task<int> GetCantidadCandidatosInactivosAsync(int partidoId);

        // Alianzas
        Task<List<AlianzaPolitica>> GetAlianzasByPartidoAsync(int partidoId);
        Task<int> GetCantidadAlianzasAsync(int partidoId);

        // Solicitudes de alianzas
        Task<List<SolicitudAlianza>> GetSolicitudesAlianzasPendientesAsync(int partidoId);
        Task<int> GetCantidadSolicitudesPendientesAsync(int partidoId);

        // Asignaciones de candidatos
        Task<List<AsignacionCandidatoPuesto>> GetCandidatosAsignadosAsync(int partidoId);
        Task<int> GetCantidadCandidatosAsignadosAsync(int partidoId);

        // Puestos electivos
        Task<int> GetCantidadPuestosElectivosTotalesAsync();

        // Validaciones
        Task<bool> EsDirigenteDelPartidoAsync(int usuarioId, int partidoId);
    }
}

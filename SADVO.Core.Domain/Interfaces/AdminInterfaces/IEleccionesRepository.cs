// Interfaces/AdminInterfaces/IEleccionesRepository.cs
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Entities.Dirigente;
using Sadvo.Core.Domain.Entities.Elector;

namespace Sadvo.Core.Domain.Interfaces.AdminInterfaces
{
    public interface IEleccionesRepository : IGenericRepository<Eleccion>
    {
        // Métodos existentes
        Task<Eleccion?> GetByIdWithVotosAsync(int id);
        Task<bool> EstaFinalizada(int Id);
        Task<bool> HayPuestosElectivosActivos();
        Task<bool> HayPartidoPolitico(int Id);
        Task<List<Eleccion>> GetAllWithVotosAsync();
        Task<List<Eleccion>> GetEleccionesOrdenadas();
        Task<bool> HayEleccionActiva();
        Task<Eleccion?> GetEleccionActiva();
        Task<bool> FinalizarEleccion(int eleccionId);
        Task<int> GetCantidadPartidosParticipantes(int eleccionId);
        Task<int> GetCantidadPuestosDisputados(int eleccionId);
        Task<bool> HaySuficientesPartidosActivos();
        Task<List<PartidoPolitico>> GetPartidosActivos();
        Task<List<PuestoElectivo>> GetPuestosElectivosActivos();
        Task<List<PuestoElectivo>> GetPuestosSinCandidatosPorPartido(int partidoId);
        Task<List<Voto>> GetVotosPorEleccion(int eleccionId);
        Task<bool> CrearEleccion(string nombre, DateOnly fechaRealizacion);
        Task<int> GetTotalVotosPorPuesto(int eleccionId, int puestoElectivoId);
        Task<int> GetVotosPorCandidato(int eleccionId, int candidatoId, int puestoElectivoId);
    }
}
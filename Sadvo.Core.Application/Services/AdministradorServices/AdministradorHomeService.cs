using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Interfaces.AdministradorInterfaces;
using Sadvo.Core.Domain.Interfaces.AdminInterfaces;

namespace Sadvo.Core.Application.Services.AdministradorServices
{
    public class AdministradorHomeService : IAdministradorHomeService
    {
        private readonly IAdministradorHomeRepository _administradorHomeRepository;

        public AdministradorHomeService(IAdministradorHomeRepository administradorHomeRepository)
        {
            _administradorHomeRepository = administradorHomeRepository;
        }

        public async Task<AdministradorHomeDto> GetDashboardDataAsync()
        {
            try
            {
                var años = await GetAñosConEleccionesAsync();
                var añoMasReciente = años.Any() ? años.First() : DateTime.Now.Year;

                return new AdministradorHomeDto
                {
                    AñosDisponibles = años,
                    AñoMasReciente = añoMasReciente,
                    ResumenElectoral = new List<ResumenElectoralDto>()
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener datos del dashboard del administrador", ex);
            }
        }

        public async Task<List<int>> GetAñosConEleccionesAsync()
        {
            try
            {
                return await _administradorHomeRepository.GetAñosConEleccionesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener años con elecciones", ex);
            }
        }

        public async Task<List<ResumenElectoralDto>> GetResumenElectoralPorAñoAsync(int año)
        {
            try
            {
                var elecciones = await _administradorHomeRepository.GetEleccionesPorAñoAsync(año);
                var resumenElectoral = new List<ResumenElectoralDto>();

                foreach (var eleccion in elecciones)
                {
                    var cantidadPartidos = await _administradorHomeRepository.GetCantidadPartidosParticipantesAsync(eleccion.Id);
                    var cantidadCandidatos = await _administradorHomeRepository.GetCantidadCandidatosParticipantesAsync(eleccion.Id);
                    var totalVotos = await _administradorHomeRepository.GetCantidadTotalVotosAsync(eleccion.Id);

                    resumenElectoral.Add(new ResumenElectoralDto
                    {
                        EleccionId = eleccion.Id,
                        NombreEleccion = eleccion.Nombre ?? "Sin nombre",
                        FechaEleccion = eleccion.FechaRealizacion,
                        CantidadPartidos = cantidadPartidos,
                        CantidadCandidatos = cantidadCandidatos,
                        TotalVotos = totalVotos,
                        EstadoEleccion = ObtenerEstadoTexto(eleccion.Estado)
                    });
                }

                return resumenElectoral.OrderByDescending(r => r.FechaEleccion).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener resumen electoral del año {año}", ex);
            }
        }

        private string ObtenerEstadoTexto(Sadvo.Core.Domain.Enums.EstadoEleccion estado)
        {
            return estado switch
            {
                Sadvo.Core.Domain.Enums.EstadoEleccion.EnProceso => "En Proceso",
                Sadvo.Core.Domain.Enums.EstadoEleccion.Finalizada => "Finalizada",
                _ => "Desconocido"
            };
        }
    }
}

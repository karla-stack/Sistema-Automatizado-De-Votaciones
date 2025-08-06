using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadvo.Infrastructure.Persistence.Repositories.AdministradorRepository
{
    using global::Sadvo.Core.Domain.Entities.Administrador;
    using global::Sadvo.Core.Domain.Interfaces.AdminInterfaces;
    using global::Sadvo.Infrastructure.Persistence.Contexts;
    using Microsoft.EntityFrameworkCore;

    namespace Sadvo.Infrastructure.Persistence.Repositories.AdminRepositories
    {
        public class AdministradorHomeRepository : IAdministradorHomeRepository
        {
            private readonly AppDbContext _context;

            public AdministradorHomeRepository(AppDbContext context)
            {
                _context = context;
            }

            public async Task<List<int>> GetAñosConEleccionesAsync()
            {
                try
                {
                    return await _context.Elecciones
                        .Select(e => e.FechaRealizacion.Year)
                        .Distinct()
                        .OrderByDescending(año => año)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener años con elecciones", ex);
                }
            }

            public async Task<List<Eleccion>> GetEleccionesPorAñoAsync(int año)
            {
                try
                {
                    return await _context.Elecciones
                        .Where(e => e.FechaRealizacion.Year == año)
                        .OrderByDescending(e => e.FechaRealizacion)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al obtener elecciones del año {año}", ex);
                }
            }

            public async Task<int> GetCantidadPartidosParticipantesAsync(int eleccionId)
            {
                try
                {
                    // Contar partidos únicos que tienen votos en esta elección
                    return await _context.Votos
                        .Where(v => v.EleccionId == eleccionId)
                        .Select(v => v.PartidoPoliticoId)
                        .Distinct()
                        .CountAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al contar partidos participantes", ex);
                }
            }

            public async Task<int> GetCantidadCandidatosParticipantesAsync(int eleccionId)
            {
                try
                {
                    // Contar candidatos únicos que recibieron votos en esta elección
                    return await _context.Votos
                        .Where(v => v.EleccionId == eleccionId)
                        .Select(v => v.CandidatoId)
                        .Distinct()
                        .CountAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al contar candidatos participantes", ex);
                }
            }

            public async Task<int> GetCantidadTotalVotosAsync(int eleccionId)
            {
                try
                {
                    // Contar total de votos emitidos en la elección
                    return await _context.Votos
                        .CountAsync(v => v.EleccionId == eleccionId);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al contar total de votos", ex);
                }
            }
        }
    }

    // Método alternativo si no tienes tabla Votos directa o estructura diferente
    /*
    // Si los votos están en otra estructura, ajustar así:

    public async Task<int> GetCantidadPartidosParticipantesAsync(int eleccionId)
    {
        try
        {
            // Opción 1: Si tienes AsignacionesCandidatosPuestos
            return await _context.AsignacionesCandidatosPuestos
                .Include(a => a.Candidato)
                .Where(a => a.Candidato.Votos.Any(v => v.EleccionId == eleccionId))
                .Select(a => a.PartidoPoliticoId)
                .Distinct()
                .CountAsync();

            // Opción 2: Si cuentas por candidatos asignados
            var candidatosConVotos = await _context.Candidatos
                .Where(c => c.Votos.Any(v => v.EleccionId == eleccionId))
                .Select(c => c.PartidoPoliticoId)
                .Distinct()
                .CountAsync();

            return candidatosConVotos;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al contar partidos participantes", ex);
        }
    }

    public async Task<int> GetCantidadCandidatosParticipantesAsync(int eleccionId)
    {
        try
        {
            // Contar candidatos que tienen votos en esta elección
            return await _context.Candidatos
                .CountAsync(c => c.Votos.Any(v => v.EleccionId == eleccionId));
        }
        catch (Exception ex)
        {
            throw new Exception("Error al contar candidatos participantes", ex);
        }
    }

    public async Task<int> GetCantidadTotalVotosAsync(int eleccionId)
    {
        try
        {
            // Si los votos están relacionados a candidatos
            return await _context.Votos
                .Where(v => v.Candidato.AsignacionesPuestos.Any() && v.EleccionId == eleccionId)
                .CountAsync();

            // O si tienes una relación directa
            return await _context.Votos
                .CountAsync(v => v.EleccionId == eleccionId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al contar total de votos", ex);
        }
    }
    */
}

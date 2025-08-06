using Microsoft.EntityFrameworkCore;
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Entities.Dirigente;
using Sadvo.Core.Domain.Enums;
using Sadvo.Core.Domain.Interfaces.DirigenteInterfaces;
using Sadvo.Infrastructure.Persistence.Contexts;

namespace Sadvo.Infrastructure.Persistence.Repositories.DirigenteRepository
{
    public class CandidatoRepository : GenericRepository<Candidato>, ICandidatoRepository
    {
        private AppDbContext _appDbContext;
        public CandidatoRepository(AppDbContext context) : base(context)
        {
            _appDbContext = context;
        }

        public async Task<bool> HayEleccionActiva()
        {
            return await _context.Set<Eleccion>()
                .Where(p => p.Estado == EstadoEleccion.EnProceso)
                .AnyAsync();
        }

        public async Task<bool> EstaAsociadoPuesto(int candidatoId)
        {
            return await _context.Set<AsignacionCandidatoPuesto>()
                .AnyAsync(acp => acp.CandidatoId == candidatoId);
        }

        public async Task<Candidato?> GetByIdAsyncPartido(int id)
        {
            var candidato = await _appDbContext.Set<Candidato>()
                .Include(c => c.PartidoPolitico)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (candidato != null)
            {
                // Cargar manualmente las asignaciones con puestos
                await _appDbContext.Entry(candidato)
                    .Collection(c => c.PuestosAsignados)
                    .Query()
                    .Include(pa => pa.PuestoElectivo)
                    .LoadAsync();
            }

            return candidato;
        }

        public async Task<List<Candidato>> GetListCandidatos()
        {
            var candidatos = await _appDbContext.Set<Candidato>()
                .Include(c => c.PartidoPolitico)
                .OrderBy(c => c.Apellido)
                .ThenBy(c => c.Nombre)
                .ToListAsync();

            // Cargar puestos asignados para cada candidato
            foreach (var candidato in candidatos)
            {
                await _appDbContext.Entry(candidato)
                    .Collection(c => c.PuestosAsignados)
                    .Query()
                    .Include(pa => pa.PuestoElectivo)
                    .LoadAsync();
            }

            return candidatos;
        }

        public async Task<List<AsignacionCandidatoPuesto>> ListaAsignacionCandidatoPuesto(int partidoDirigenteId)
        {
            Console.WriteLine($"=== DEBUG REPOSITORY ===");
            Console.WriteLine($"Filtrando por partido: {partidoDirigenteId}");

            var resultado = await _context.Set<AsignacionCandidatoPuesto>()
                .Include(a => a.Candidato)
                .Include(a => a.PuestoElectivo)
                .Include(a => a.PartidoPolitico)
                .Where(a => a.Candidato != null &&
                            a.Candidato.Estado == Actividad.Activo &&
                            a.PuestoElectivo != null &&
                            a.PuestoElectivo.Estado == Actividad.Activo &&
                            a.PartidoPoliticoId == partidoDirigenteId)
                .OrderBy(a => a.Candidato.Nombre)
                .ThenBy(a => a.Candidato.Apellido)
                .ToListAsync();

            Console.WriteLine($"Query ejecutada - Resultados: {resultado.Count}");
            return resultado;
        }

        // ✅ MÉTODO CORREGIDO: Incluye las asignaciones de puestos
        public async Task<List<Candidato>> GetCandidatosByPartido(int partidoId)
        {
            var candidatos = await _context.Set<Candidato>()
                .Include(c => c.PartidoPolitico)
                .Where(c => c.PartidoPoliticoId == partidoId &&
                            c.Estado == Actividad.Activo)
                .OrderBy(c => c.Nombre)
                .ThenBy(c => c.Apellido)
                .ToListAsync();

            // Cargar las asignaciones de puestos para cada candidato
            foreach (var candidato in candidatos)
            {
                await _appDbContext.Entry(candidato)
                    .Collection(c => c.PuestosAsignados)
                    .Query()
                    .Include(pa => pa.PuestoElectivo)
                    .Where(pa => pa.PartidoPoliticoId == partidoId) // Solo asignaciones del partido actual
                    .LoadAsync();
            }

            return candidatos;
        }

        // ✅ NUEVO MÉTODO: Más eficiente con una sola query
        public async Task<List<CandidatoConPuestoDto>> GetCandidatosConPuestosByPartido(int partidoId)
        {
            var query = from c in _context.Set<Candidato>()
                        where c.PartidoPoliticoId == partidoId && c.Estado == Actividad.Activo
                        join acp in _context.Set<AsignacionCandidatoPuesto>()
                            on c.Id equals acp.CandidatoId into asignaciones
                        from asignacion in asignaciones.Where(a => a.PartidoPoliticoId == partidoId).DefaultIfEmpty()
                        join pe in _context.Set<PuestoElectivo>()
                            on asignacion.PuestoElectivoId equals pe.Id into puestos
                        from puesto in puestos.DefaultIfEmpty()
                        select new CandidatoConPuestoDto
                        {
                            Id = c.Id,
                            Nombre = c.Nombre,
                            Apellido = c.Apellido,
                            Foto = c.Foto,
                            Estado = c.Estado,
                            PartidoPoliticoId = c.PartidoPoliticoId,
                            PuestoAsociado = puesto != null ? puesto.Nombre : "Sin puesto asociado"
                        };

            return await query.ToListAsync();
        }

        // ✅ MÉTODO AUXILIAR: Para verificar si candidato tiene asignación específica
        public async Task<string?> ObtenerPuestoAsignadoPorPartido(int candidatoId, int partidoId)
        {
            var asignacion = await _context.Set<AsignacionCandidatoPuesto>()
                .Include(a => a.PuestoElectivo)
                .FirstOrDefaultAsync(a => a.CandidatoId == candidatoId &&
                                         a.PartidoPoliticoId == partidoId);

            return asignacion?.PuestoElectivo?.Nombre;
        }
    }
}
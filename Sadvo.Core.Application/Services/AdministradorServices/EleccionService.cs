using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Helpers;
using Sadvo.Core.Application.Interfaces.AdministradorInterfaces;
using Sadvo.Core.Domain.Enums;
using Sadvo.Core.Domain.Interfaces.AdminInterfaces;

namespace Sadvo.Core.Application.Services.AdministradorServices
{
    public class EleccionService : IEleccionService
    {
        private readonly IEleccionesRepository _repository;

        public EleccionService(IEleccionesRepository repository)
        {
            _repository = repository;
        }

        // Obtener lista de elecciones ordenadas
        public async Task<List<EleccionResumenDto>> GetListAsync()
        {
            try
            {
                var elecciones = await _repository.GetEleccionesOrdenadas();
                var lista = new List<EleccionResumenDto>();

                foreach (var eleccion in elecciones)
                {
                    var cantPartidos = await _repository.GetCantidadPartidosParticipantes(eleccion.Id);
                    var cantPuestos = await _repository.GetCantidadPuestosDisputados(eleccion.Id);

                    lista.Add(new EleccionResumenDto
                    {
                        Id = eleccion.Id,
                        Nombre = eleccion.Nombre,
                        Fecha = eleccion.FechaRealizacion,
                        CantPuestos = cantPuestos,
                        CantPartidos = cantPartidos,
                        EstaFinalizada = eleccion.Estado == EstadoEleccion.Finalizada,
                        EstaActiva = eleccion.Estado == EstadoEleccion.EnProceso
                    });
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las elecciones", ex);
            }
        }

        // Fix for CS1061: Adjust the code to correctly access the `CandidatoId` property from the `Key` of the grouping.
        public async Task<List<EleccionDetalleDto>> GetByIdResult(int id)
        {
            try
            {
                var votos = await _repository.GetVotosPorEleccion(id);
                if (!votos.Any())
                    return new List<EleccionDetalleDto>();

                var resultado = votos
                    .GroupBy(v => new {
                        v.PuestoElectivoId,
                        NombrePuesto = v.PuestoElectivo?.Nombre ?? "Puesto desconocido"
                    })
                    .Select(puestoGroup =>
                    {
                        var candidatos = puestoGroup
                            .GroupBy(v => new
                            {
                                v.CandidatoId,
                                NombreCandidato = $"{v.Candidato?.Nombre ?? "N/A"} {v.Candidato?.Apellido ?? "N/A"}",
                                Partido = v.PartidoPolitico?.Nombre ?? "N/A",
                                SiglasPartido = v.PartidoPolitico?.Siglas ?? ""
                            })
                            .Select(candidatoGroup => new CandidatoResultadoDto
                            {
                                CandidatoId = candidatoGroup.Key.CandidatoId ?? 0,
                                NombreCandidato = candidatoGroup.Key.NombreCandidato,
                                Partido = candidatoGroup.Key.Partido,
                                SiglasPartido = candidatoGroup.Key.SiglasPartido,
                                CantidadVotos = candidatoGroup.Count()
                            })
                            .ToList();

                        // CORREGIR: Calcular porcentaje después del agrupamiento
                        var totalVotosPuesto = candidatos.Sum(c => c.CantidadVotos);

                        candidatos.ForEach(c =>
                        {
                            c.Porcentaje = totalVotosPuesto > 0
                                ? Math.Round((c.CantidadVotos * 100.0) / totalVotosPuesto, 2)
                                : 0;
                        });

                        return new EleccionDetalleDto
                        {
                            PuestoElectivoId = puestoGroup.Key.PuestoElectivoId,
                            NombrePuesto = puestoGroup.Key.NombrePuesto,
                            Candidatos = candidatos.OrderByDescending(c => c.CantidadVotos).ToList()
                        };
                    })
                    .OrderBy(r => r.NombrePuesto)
                    .ToList();

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el resumen de la elección", ex);
            }
        }


        // Verificar si hay elección activa
        public async Task<bool> HayEleccionActiva()
        {
            return await _repository.HayEleccionActiva();
        }

        // Obtener elección activa
        public async Task<EleccionDto?> GetEleccionActiva()
        {
            try
            {
                var eleccion = await _repository.GetEleccionActiva();
                if (eleccion == null) return null;

                return new EleccionDto
                {
                    Id = eleccion.Id,
                    Nombre = eleccion.Nombre,
                    FechaRealizacion = eleccion.FechaRealizacion,
                    Estado = eleccion.Estado
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la elección activa", ex);
            }
        }

        // Finalizar elección
        public async Task<bool> FinalizarEleccion(int eleccionId)
        {
            try
            {
                return await _repository.FinalizarEleccion(eleccionId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al finalizar la elección", ex);
            }
        }

        // Validaciones para crear nueva elección
        public async Task<ResultadoValidacionEleccion> ValidarCreacionEleccion()
        {
            try
            {
                var resultado = new ResultadoValidacionEleccion { EsValida = true };

                // 1. Verificar que no hay elección activa
                if (await _repository.HayEleccionActiva())
                {
                    resultado.EsValida = false;
                    resultado.Mensajes.Add("Ya existe una elección activa. No se puede crear otra.");
                    return resultado;
                }

                // 2. Verificar que hay puestos electivos activos
                if (!await _repository.HayPuestosElectivosActivos())
                {
                    resultado.EsValida = false;
                    resultado.Mensajes.Add("No hay puestos electivos activos para crear una elección.");
                    return resultado;
                }

                // 3. Verificar que hay al menos 2 partidos activos
                if (!await _repository.HaySuficientesPartidosActivos())
                {
                    resultado.EsValida = false;
                    resultado.Mensajes.Add("No hay suficientes partidos políticos para realizar una elección.");
                    return resultado;
                }

                // 4. Verificar que cada partido tiene candidatos para todos los puestos
                var partidos = await _repository.GetPartidosActivos();
                var puestos = await _repository.GetPuestosElectivosActivos();

                foreach (var partido in partidos)
                {
                    var puestosSinCandidatos = await _repository.GetPuestosSinCandidatosPorPartido(partido.Id);

                    if (puestosSinCandidatos.Any())
                    {
                        resultado.EsValida = false;
                        var nombresPuestos = string.Join(", ", puestosSinCandidatos.Select(p => p.Nombre));
                        resultado.Mensajes.Add(
                            $"El partido político {partido.Nombre} ({partido.Siglas}) no tiene candidatos registrados para los siguientes puestos electivos: {nombresPuestos}."
                        );
                    }
                }

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al validar la creación de elección", ex);
            }
        }

        // Crear nueva elección
        public async Task<bool> AddAsync(EleccionDto entity)
        {
            try
            {
                // Validar antes de crear
                var validacion = await ValidarCreacionEleccion();
                if (!validacion.EsValida)
                {
                    return false;
                }

                return await _repository.CrearEleccion(entity.Nombre ?? "", entity.FechaRealizacion);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al agregar la elección", ex);
            }
        }

        // Obtener elección por ID
        public async Task<EleccionDto?> GetById(int id)
        {
            try
            {
                var eleccion = await _repository.GetByIdAsync(id);
                if (eleccion == null) return null;

                return new EleccionDto
                {
                    Id = eleccion.Id,
                    Nombre = eleccion.Nombre,
                    FechaRealizacion = eleccion.FechaRealizacion,
                    Estado = eleccion.Estado
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la elección", ex);
            }
        }
    }
}
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Helpers;
using Sadvo.Core.Application.Interfaces;
using Sadvo.Core.Application.Interfaces.DirigenteInterfaces;
using Sadvo.Core.Domain.Entities.Dirigente;
using Sadvo.Core.Domain.Interfaces.DirigenteInterfaces;

public class AsignarCandidatoService : IAsignarCandidatoService
{
    private readonly IAsignacionCandidatoRepository _repository;
    private readonly IValidateUserSession _session;

    public AsignarCandidatoService(IAsignacionCandidatoRepository repository, IValidateUserSession session)
    {
        _repository = repository;
        _session = session;
    }

    public async Task<bool> AddAsync(int puestoElectivoId, int candidatoId)
    {
        try
        {
            var session = _session.GetUserSession();

            if (session?.PartidoPoliticoId == null)
            {
                throw new Exception("Dirigente sin partido asignado");
            }

            var partidoDirigenteId = session.PartidoPoliticoId.Value;

            // Validar que el candidato puede ser asignado a ese puesto
            var esValido = await _repository.ValidarCandidatoParaPuesto(candidatoId, puestoElectivoId, partidoDirigenteId);
            if (!esValido)
            {
                throw new Exception("Este candidato no puede ser asignado a este puesto según las reglas de alianzas.");
            }

            var asignacion = new AsignacionCandidatoPuesto
            {
                CandidatoId = candidatoId,
                PuestoElectivoId = puestoElectivoId,
                PartidoPoliticoId = partidoDirigenteId
            };

            var entityResult = await _repository.AddAsync(asignacion);
            return entityResult != null;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al asignar candidato.", ex);
        }
    }

    public async Task<ResultadoOperacion> ValidacionEleccionActiva()
    {
        if (await _repository.HayEleccionActiva())
        {
            return new ResultadoOperacion
            {
                Exito = false,
                Mensaje = "Accion denegada existe una seleccion activa"
            };
        }
        return new ResultadoOperacion
        {
            Exito = true,
            Mensaje = ""
        };
    }


    public async Task<List<PuestoCandidatoDto>> GetListAsync()
    {
        try
        {
            var session = _session.GetUserSession();

            if (session?.PartidoPoliticoId == null || session.PartidoPoliticoId == 0)
            {
                Console.WriteLine("Dirigente sin partido - devolviendo lista vacía");
                return new List<PuestoCandidatoDto>();
            }

            var partidoDirigenteId = session.PartidoPoliticoId.Value;
            Console.WriteLine($"Obteniendo asignaciones para partido: {partidoDirigenteId}");

            var lista = await _repository.ListaAsignacionCandidatoPuesto(partidoDirigenteId);

            var listaFiltrada = lista
                .Select(a => new PuestoCandidatoDto
                {
                    Id = a.Id,
                    Nombre = a.Candidato.Nombre,
                    Apellido = a.Candidato.Apellido,
                    PuestoElectivo = a.PuestoElectivo.Nombre,
                    CandidatoId = a.CandidatoId,
                    PuestoElectivoId = a.PuestoElectivoId
                })
                .ToList();

            Console.WriteLine($"Asignaciones encontradas: {listaFiltrada.Count}");
            return listaFiltrada;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR GetListAsync: {ex.Message}");
            throw new Exception("Error al obtener la lista de asignaciones de candidatos", ex);
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var asignacion = await _repository.GetByIdAsync(id);
            if (asignacion == null)
            {
                return false;
            }
            await _repository.DeleteAsync(id);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al eliminar la asignación de candidato", ex);
        }
    }

    public async Task<List<CandidatoDto>> ObtenerCandidato()
    {
        try
        {
            var session = _session.GetUserSession();


            if (session?.PartidoPoliticoId == null)
            {
                return new List<CandidatoDto>();
            }

            int partidoDirigenteId = session.PartidoPoliticoId.Value;

            var lista = await _repository.ObtenerCandidatosDisponibles(partidoDirigenteId);

            if (lista == null)
            {
                return new List<CandidatoDto>();
            }

            var listaDto = lista.Select(usuario => new CandidatoDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
            }).ToList();

            return listaDto;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los candidatos", ex);
        }
    }

    public async Task<List<PuestoElectivoDto>> ObtenerPuestoElectivo()
    {
        try
        {
            var session = _session.GetUserSession();

            if (session?.PartidoPoliticoId == null)
            {
                return new List<PuestoElectivoDto>();
            }

            int partidoDirigenteId = session.PartidoPoliticoId.Value;

            var lista = await _repository.ObtenerPuestosDisponibles(partidoDirigenteId);

            if (lista == null)
            {
                return new List<PuestoElectivoDto>();
            }

            var listaDto = lista.Select(puesto => new PuestoElectivoDto
            {
                Id = puesto.Id,
                Nombre = puesto.Nombre
            }).ToList();

            return listaDto;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los puestos electivos", ex);
        }
    }

    public async Task<AsignacionCandidatoDto?> GetById(int id)
    {
        try
        {
            var resultado = await _repository.GetByIdAsync(id);
            if (resultado == null)
            {
                return null;
            }

            AsignacionCandidatoDto asignacion = new()
            {
                Id = resultado.Id,
                CandidatoId = resultado.CandidatoId,
                PuestoElectivoId = resultado.PuestoElectivoId,
                Nombre = resultado.Candidato?.Nombre ?? "",
                Apellido = resultado.Candidato?.Apellido ?? "",
                PuestoAsignado = resultado.PuestoElectivo?.Nombre ?? "",
                PartidoPoliticoId = resultado.PartidoPoliticoId 
            };

            return asignacion;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener la asignación por id", ex);
        }
    }
}
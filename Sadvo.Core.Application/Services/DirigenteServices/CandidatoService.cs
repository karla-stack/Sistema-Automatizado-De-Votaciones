using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Helpers;
using Sadvo.Core.Application.Interfaces;
using Sadvo.Core.Application.Interfaces.DirigenteInterfaces;
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Entities.Dirigente;
using Sadvo.Core.Domain.Enums;
using Sadvo.Core.Domain.Interfaces.DirigenteInterfaces;

namespace Sadvo.Core.Application.Services.DirigenteServices
{
    public class CandidatoService : ICandidatoService
    {
        private readonly ICandidatoRepository _repository;
        private readonly IValidateUserSession _session;
        //private readonly IMapper _mapper;
        public CandidatoService(ICandidatoRepository repository, IValidateUserSession session)
        {
            _repository = repository;
            _session = session;
            //_mapper = mapper;
        }

        public async Task<bool?> AddAsync(CandidatoDto candidato)
        {
            try
            {
                var currentUser = _session.GetUserSession();

                if (currentUser?.PartidoPoliticoId == null)
                {
                    throw new Exception("El usuario actual no tiene un PartidoPoliticoId asignado.");
                }

                Candidato entity = new()
                {
                    Id = 0,
                    Nombre = candidato.Nombre ?? string.Empty,
                    Apellido = candidato.Apellido ?? string.Empty,
                    Foto = candidato.Foto ?? string.Empty,
                    Estado = Actividad.Activo,
                    PartidoPoliticoId = currentUser.PartidoPoliticoId.Value // ← USAR EL PARTIDO DEL DIRIGENTE
                };

                Candidato? entitydto = await _repository.AddAsync(entity);

                if (entitydto == null)
                {
                    return null;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al agregar candidato", ex);
            }
        }

        public async Task<bool?> UpdateAsync(int id, CandidatoDto candidato)
        {
            try
            {
                var entityExistente = await _repository.GetByIdAsync(id);

                if (entityExistente == null)
                {
                    return null;
                }

                // Actualizar propiedades
                entityExistente.Nombre = candidato.Nombre.Trim();
                entityExistente.Apellido = candidato.Apellido.Trim();
                entityExistente.Foto = candidato.Foto?.Trim();
                entityExistente.Estado = candidato.Estado;
                entityExistente.PartidoPoliticoId = candidato.PartidoPoliticoId;

                 await _repository.UpdateAsync(id, entityExistente);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el candidato", ex);
            }
        }

        public async Task<List<Candidato>> GetAllListAsync()
        {
            try
            {
                var lista = await _repository.GetAllList();
                if (lista == null || !lista.Any())
                {
                    return new List<Candidato>();
                }

                return lista.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de puestos electivos", ex);
            }
        }


        public async Task<bool?> CambiarEstadoAsync(int id, CandidatoDto candidato)
        {
            try
            {

                var entity = await _repository.GetByIdAsync(id);

                if (entity == null)
                {
                    return false;
                }

                entity.Estado = candidato.Estado;

                Candidato? entitydto = await _repository.UpdateAsync(id, entity);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar el estado del puesto electivo", ex);
            }
        }

        public async Task<CandidatoDto?> GetById(int id)
        {
            try
            {
                var resultado = await _repository.GetByIdAsyncPartido(id);
                if (resultado == null)
                { return null; }

                CandidatoDto candidato = new()
                {
                    Id = resultado.Id,
                    Nombre = resultado.Nombre ?? string.Empty,
                    Apellido = resultado.Apellido ?? string.Empty,
                    Foto = resultado.Foto ?? string.Empty,
                    Estado = resultado.Estado,
                    PartidoPoliticoId = resultado.PartidoPoliticoId,
                    PuestoAsociado = resultado.PuestosAsignados?.Any() == true
                    ? resultado.PuestosAsignados.First().PuestoElectivo?.Nombre ?? "Sin puesto asociado"
                    : "Sin puesto asociado"

                };
                return candidato;

            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener candidato por id", ex);
            }
        }

        public async Task<ResultadoOperacion> ValidacionEleccionActiva()
        {
            if (await _repository.HayEleccionActiva())
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Accion denegada existe una elección activa"
                };

            }
            return new ResultadoOperacion
            {
                Exito = true,
                Mensaje = ""
            };
        }

        // OBTENER CANDIDATOS POR PARTIDO
        public async Task<List<PuestoCandidatoDto>> GetListCandidato()
        {
            try
            {
                var session = _session.GetUserSession();

                Console.WriteLine($"=== DEBUG SESSION ===");
                Console.WriteLine($"Usuario: {session?.NombreUsuario}");
                Console.WriteLine($"PartidoPoliticoId: {session?.PartidoPoliticoId}");

                if (session?.PartidoPoliticoId == null || session.PartidoPoliticoId == 0)
                {
                    Console.WriteLine("DIRIGENTE SIN PARTIDO - Devolviendo lista vacía");
                    return new List<PuestoCandidatoDto>();
                }

                var partidoDirigenteId = session.PartidoPoliticoId.Value;
                Console.WriteLine($"Buscando asignaciones para partido: {partidoDirigenteId}");

                var lista = await _repository.ListaAsignacionCandidatoPuesto(partidoDirigenteId);

                Console.WriteLine($"Registros encontrados en BD: {lista.Count}");
                foreach (var item in lista)
                {
                    Console.WriteLine($"- Candidato: {item.Candidato?.Nombre} - Partido Asignación: {item.PartidoPoliticoId}");
                }

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

                Console.WriteLine($"Registros después del mapeo: {listaFiltrada.Count}");
                return listaFiltrada;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                throw new Exception("Error al obtener la lista de asignaciones de candidatos", ex);
            }
        }

        // En tu CandidatoService
        public async Task<List<CandidatoDto>> GetListCandidatosByPartido(int partidoId)
        {
            try
            {
                var candidatos = await _repository.GetCandidatosByPartido(partidoId);

                // Map the list of Candidato to CandidatoDto
                var candidatoDtos = candidatos.Select(c => new CandidatoDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Apellido = c.Apellido,
                    Foto = c.Foto ?? string.Empty,
                    Estado = c.Estado,
                    PartidoPoliticoId = c.PartidoPoliticoId,
                    PuestoAsociado = c.PuestosAsignados?.Any() == true
                        ? c.PuestosAsignados.First().PuestoElectivo?.Nombre ?? "Sin puesto asociado"
                        : "Sin puesto asociado"
                }).ToList();

                return candidatoDtos;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener candidatos por partido", ex);
            }
        }


    }
}

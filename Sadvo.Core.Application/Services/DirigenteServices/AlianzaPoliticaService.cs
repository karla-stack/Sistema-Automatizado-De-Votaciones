using Microsoft.AspNetCore.Http;
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Helpers;
using Sadvo.Core.Application.Interfaces;
using Sadvo.Core.Application.Interfaces.DirigenteInterfaces;
using Sadvo.Core.Domain.Enums;
using Sadvo.Core.Domain.Interfaces.DirigenteInterfaces;

namespace Sadvo.Core.Application.Services.DirigenteServices
{
    public class AlianzaPoliticaService : IAlianzaPoliticaService
    {
        private readonly IAlianzaPoliticaRepository _repository;
        private readonly IValidateUserSession _session;

        public AlianzaPoliticaService(IAlianzaPoliticaRepository repository, IValidateUserSession session)
        {
            _repository = repository;
            _session = session;
        }

        public async Task<List<SolicitudAlianzaPendienteDto>> GetSolicitudesPendientes()
        {
            try
            {
                var session = _session.GetUserSession();

                if (session?.PartidoPoliticoId == null || session.PartidoPoliticoId == 0)
                {
                    return new List<SolicitudAlianzaPendienteDto>();
                }

                var partidoReceptorId = session.PartidoPoliticoId.Value;
                var solicitudes = await _repository.GetSolicitudesPendientesByPartido(partidoReceptorId);

                var solicitudesDto = solicitudes.Select(s => new SolicitudAlianzaPendienteDto
                {
                    Id = s.Id,
                    PartidoSolicitanteId = s.PartidoSolicitanteId,
                    NombrePartidoSolicitante = s.PartidoSolicitante?.Nombre ?? "",
                    SiglasPartidoSolicitante = s.PartidoSolicitante?.Siglas ?? "",
                    FechaSolicitud = s.FechaSolicitud,
                    Estado = s.Estado
                }).ToList();

                return solicitudesDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las solicitudes pendientes", ex);
            }
        }

        public async Task<ResultadoOperacion> ValidacionEleccionActiva()
        {
            if (await _repository.HayEleccionActiva())
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "No se pueden aceptar o rechazar solicitudes mientras exista una elección activa"
                };
            }
            return new ResultadoOperacion
            {
                Exito = true,
                Mensaje = ""
            };
        }

        public async Task<bool> AceptarSolicitud(int solicitudId)
        {
            try
            {
                return await _repository.ActualizarEstadoSolicitud(solicitudId, EstadoSolicitudAlianza.Aceptada);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al aceptar la solicitud", ex);
            }
        }

        public async Task<bool> RechazarSolicitud(int solicitudId)
        {
            try
            {
                return await _repository.ActualizarEstadoSolicitud(solicitudId, EstadoSolicitudAlianza.Rechazada);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al rechazar la solicitud", ex);
            }
        }

        public async Task<List<SolicitudAlianzaRealizadaDto>> GetSolicitudesRealizadas()
        {
            try
            {
                var session = _session.GetUserSession();

                if (session?.PartidoPoliticoId == null || session.PartidoPoliticoId == 0)
                {
                    return new List<SolicitudAlianzaRealizadaDto>();
                }

                var partidoSolicitanteId = session.PartidoPoliticoId.Value;
                var solicitudes = await _repository.GetSolicitudesRealizadasByPartido(partidoSolicitanteId);

                var solicitudesDto = solicitudes.Select(s => new SolicitudAlianzaRealizadaDto
                {
                    Id = s.Id,
                    PartidoReceptorId = s.PartidoReceptorId,
                    NombrePartidoReceptor = s.PartidoReceptor?.Nombre ?? "",
                    SiglasPartidoReceptor = s.PartidoReceptor?.Siglas ?? "",
                    FechaSolicitud = s.FechaSolicitud,
                    Estado = s.Estado
                }).ToList();

                return solicitudesDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las solicitudes realizadas", ex);
            }
        }

        public async Task<bool> EliminarSolicitudRealizada(int solicitudId)
        {
            try
            {
                var session = _session.GetUserSession();

                if (session?.PartidoPoliticoId == null || session.PartidoPoliticoId == 0)
                {
                    return false;
                }

                var partidoSolicitanteId = session.PartidoPoliticoId.Value;

                var puedeEliminar = await _repository.PuedeEliminarSolicitud(solicitudId, partidoSolicitanteId);
                if (!puedeEliminar)
                {
                    return false;
                }

                return await _repository.EliminarSolicitud(solicitudId, partidoSolicitanteId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la solicitud", ex);
            }
        }

        public async Task<bool> PuedeEliminarSolicitud(int solicitudId)
        {
            try
            {
                var session = _session.GetUserSession();

                if (session?.PartidoPoliticoId == null || session.PartidoPoliticoId == 0)
                {
                    return false;
                }

                var partidoSolicitanteId = session.PartidoPoliticoId.Value;
                return await _repository.PuedeEliminarSolicitud(solicitudId, partidoSolicitanteId);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<PartidoDisponibleDto>> GetPartidosDisponiblesParaSolicitud()
        {
            try
            {
                var session = _session.GetUserSession();

                if (session?.PartidoPoliticoId == null || session.PartidoPoliticoId == 0)
                {
                    return new List<PartidoDisponibleDto>();
                }

                var partidoSolicitanteId = session.PartidoPoliticoId.Value;
                var partidos = await _repository.GetPartidosDisponiblesParaSolicitud(partidoSolicitanteId);

                return partidos.Select(p => new PartidoDisponibleDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Siglas = p.Siglas
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener partidos disponibles", ex);
            }
        }

        public async Task<bool> CrearSolicitudAlianza(int partidoReceptorId)
        {
            try
            {
                var session = _session.GetUserSession();

                if (session?.PartidoPoliticoId == null || session.PartidoPoliticoId == 0)
                {
                    return false;
                }

                var partidoSolicitanteId = session.PartidoPoliticoId.Value;
                return await _repository.CrearSolicitudAlianza(partidoSolicitanteId, partidoReceptorId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear la solicitud de alianza", ex);
            }
        }

        public async Task<List<AlianzaActivaDto>> GetAlianzasActivas()
        {
            try
            {
                var session = _session.GetUserSession();

                if (session?.PartidoPoliticoId == null || session.PartidoPoliticoId == 0)
                {
                    return new List<AlianzaActivaDto>();
                }

                var partidoId = session.PartidoPoliticoId.Value;
                var alianzas = await _repository.GetAlianzasActivas(partidoId);

                return alianzas.Select(a => new AlianzaActivaDto
                {
                    Id = a.Id,
                    NombrePartidoAliado = a.PartidoSolicitanteId == partidoId ?
                                         a.PartidoReceptor?.Nombre : a.PartidoSolicitante?.Nombre,
                    SiglasPartidoAliado = a.PartidoSolicitanteId == partidoId ?
                                         a.PartidoReceptor?.Siglas : a.PartidoSolicitante?.Siglas,
                    FechaAceptacion = a.FechaSolicitud
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener alianzas activas", ex);
            }
        }

        // ✅ MÉTODOS ADICIONALES PARA CONFIRMACIONES (si los necesitas)
        public async Task<SolicitudAlianzaPendienteDto?> GetSolicitudPendienteById(int id)
        {
            try
            {
                var solicitud = await _repository.GetSolicitudPendienteById(id);
                if (solicitud == null) return null;

                return new SolicitudAlianzaPendienteDto
                {
                    Id = solicitud.Id,
                    PartidoSolicitanteId = solicitud.PartidoSolicitanteId,
                    NombrePartidoSolicitante = solicitud.PartidoSolicitante?.Nombre ?? "",
                    SiglasPartidoSolicitante = solicitud.PartidoSolicitante?.Siglas ?? "",
                    FechaSolicitud = solicitud.FechaSolicitud,
                    Estado = solicitud.Estado
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la solicitud pendiente", ex);
            }
        }

        public async Task<SolicitudAlianzaRealizadaDto?> GetSolicitudRealizadaById(int id)
        {
            try
            {
                var solicitud = await _repository.GetSolicitudRealizadaById(id);
                if (solicitud == null) return null;

                return new SolicitudAlianzaRealizadaDto
                {
                    Id = solicitud.Id,
                    PartidoReceptorId = solicitud.PartidoReceptorId,
                    NombrePartidoReceptor = solicitud.PartidoReceptor?.Nombre ?? "",
                    SiglasPartidoReceptor = solicitud.PartidoReceptor?.Siglas ?? "",
                    FechaSolicitud = solicitud.FechaSolicitud,
                    Estado = solicitud.Estado
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la solicitud realizada", ex);
            }
        }
    }
}
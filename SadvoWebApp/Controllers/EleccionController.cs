using Microsoft.AspNetCore.Mvc;
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Interfaces;
using Sadvo.Core.Application.Interfaces.AdministradorInterfaces;
using Sadvo.Core.Application.ViewModels.Eleccion;

namespace SADVOWebApplication.Controllers
{
    public class EleccionController : Controller
    {
        private readonly IValidateUserSession _session;
        private readonly IEleccionService _eleccionService;

        public EleccionController(IEleccionService eleccionService, IValidateUserSession session)
        {
            _session = session;
            _eleccionService = eleccionService;
        }

        // Vista principal de elecciones
        public async Task<IActionResult> EleccionView()
        {
            try
            {
                if (!_session.HasUserSession())
                {
                    return RedirectToAction("Login", "Login");
                }

                var elecciones = await _eleccionService.GetListAsync();
                var hayEleccionActiva = await _eleccionService.HayEleccionActiva();

                var lista = elecciones.Select(p => new EleccionListModeloViewModel
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Fecha = p.Fecha,
                    CantPuestos = p.CantPuestos,
                    CantPartidos = p.CantPartidos,
                    EstaFinalizada = p.EstaFinalizada,
                    EstaActiva = p.EstaActiva
                }).ToList();

                var vm = new EleccionesListViewModel
                {
                    elecciones = lista,
                    HayEleccionActiva = hayEleccionActiva
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar las elecciones";
                return View(new EleccionesListViewModel());
            }
        }

        // GET: Formulario para crear nueva elección
        public async Task<IActionResult> AddEleccion()
        {
            try
            {
                if (!_session.HasUserSession())
                {
                    return RedirectToAction("Login", "Login");
                }

                // Verificar si hay elección activa
                if (await _eleccionService.HayEleccionActiva())
                {
                    TempData["ErrorMessage"] = "No se puede crear una nueva elección mientras hay una elección activa";
                    return RedirectToAction("EleccionView");
                }

                // Validar si se puede crear elección
                var validacion = await _eleccionService.ValidarCreacionEleccion();

                var model = new EleccionViewModel
                {
                    FechaRealizacion = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                    MensajesValidacion = validacion.Mensajes,
                    PuedeCrearEleccion = validacion.EsValida
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar el formulario";
                return RedirectToAction("EleccionView");
            }
        }

        // POST: Crear nueva elección
        [HttpPost]
        public async Task<IActionResult> AddEleccion(EleccionViewModel model)
        {
            try
            {
                if (!_session.HasUserSession())
                {
                    return RedirectToAction("Login", "Login");
                }

                // Verificar si hay elección activa
                if (await _eleccionService.HayEleccionActiva())
                {
                    model.Mensaje = "No se puede crear una nueva elección mientras hay una elección activa";
                    return View(model);
                }

                // Validar antes de crear
                var validacion = await _eleccionService.ValidarCreacionEleccion();
                if (!validacion.EsValida)
                {
                    model.MensajesValidacion = validacion.Mensajes;
                    model.PuedeCrearEleccion = false;
                    return View(model);
                }

                if (ModelState.IsValid && !string.IsNullOrEmpty(model.Nombre))
                {
                    var dto = new EleccionDto
                    {
                        Id = 0,
                        Nombre = model.Nombre,
                        FechaRealizacion = model.FechaRealizacion,
                        Estado = Sadvo.Core.Domain.Enums.EstadoEleccion.EnProceso
                    };

                    var resultado = await _eleccionService.AddAsync(dto);

                    if (resultado)
                    {
                        TempData["SuccessMessage"] = "Elección creada exitosamente";
                        return RedirectToAction("EleccionView");
                    }
                    else
                    {
                        model.Mensaje = "No se pudo crear la elección. Verifique que se cumplan todos los requisitos.";
                    }
                }
                else
                {
                    model.Mensaje = "Por favor complete todos los campos obligatorios.";
                }

                // Recargar validaciones si hay error
                var nuevaValidacion = await _eleccionService.ValidarCreacionEleccion();
                model.MensajesValidacion = nuevaValidacion.Mensajes;
                model.PuedeCrearEleccion = nuevaValidacion.EsValida;

                return View(model);
            }
            catch (Exception ex)
            {
                model.Mensaje = $"Error: {ex.Message}";
                return View(model);
            }
        }

        // Ver resultados de elección finalizada
        public async Task<IActionResult> Resultados(int id)
        {
            try
            {
                if (!_session.HasUserSession())
                {
                    return RedirectToAction("Login", "Login");
                }

                var eleccion = await _eleccionService.GetById(id);
                if (eleccion == null)
                {
                    TempData["ErrorMessage"] = "Elección no encontrada";
                    return RedirectToAction("EleccionView");
                }

                if (eleccion.Estado != Sadvo.Core.Domain.Enums.EstadoEleccion.Finalizada)
                {
                    TempData["ErrorMessage"] = "Solo se pueden ver resultados de elecciones finalizadas";
                    return RedirectToAction("EleccionView");
                }

                var resultadoDto = await _eleccionService.GetByIdResult(id);

                var viewModel = new EleccionResultadosViewModel
                {
                    EleccionId = id,
                    NombreEleccion = eleccion.Nombre ?? "",
                    FechaEleccion = eleccion.FechaRealizacion,
                    Resultados = resultadoDto.Select(r => new EleccionDetalleViewModel
                    {
                        PuestoElectivoId = r.PuestoElectivoId,
                        NombrePuesto = r.NombrePuesto,
                        Candidatos = r.Candidatos.Select(c => new CandidatoResultadoViewModel
                        {
                            CandidatoId = c.CandidatoId,
                            NombreCandidato = c.NombreCandidato,
                            Partido = c.Partido,
                            SiglasPartido = c.SiglasPartido,
                            CantidadVotos = c.CantidadVotos,
                            Porcentaje = c.Porcentaje
                        }).ToList()
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar los resultados";
                return RedirectToAction("EleccionView");
            }
        }

        // GET: Confirmación para finalizar elección
        public async Task<IActionResult> ConfirmarFinalizarEleccion(int id)
        {
            try
            {
                if (!_session.HasUserSession())
                {
                    return RedirectToAction("Login", "Login");
                }

                var eleccion = await _eleccionService.GetById(id);
                if (eleccion == null)
                {
                    TempData["ErrorMessage"] = "Elección no encontrada";
                    return RedirectToAction("EleccionView");
                }

                if (eleccion.Estado != Sadvo.Core.Domain.Enums.EstadoEleccion.EnProceso)
                {
                    TempData["ErrorMessage"] = "Solo se pueden finalizar elecciones activas";
                    return RedirectToAction("EleccionView");
                }

                var model = new ConfirmarFinalizarEleccionViewModel
                {
                    EleccionId = id,
                    NombreEleccion = eleccion.Nombre ?? "",
                    FechaEleccion = eleccion.FechaRealizacion
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar la confirmación";
                return RedirectToAction("EleccionView");
            }
        }

        // POST: Finalizar elección
        [HttpPost]
        public async Task<IActionResult> FinalizarEleccion(int id)
        {
            try
            {
                if (!_session.HasUserSession())
                {
                    return RedirectToAction("Login", "Login");
                }

                var resultado = await _eleccionService.FinalizarEleccion(id);

                if (resultado)
                {
                    TempData["SuccessMessage"] = "Elección finalizada exitosamente";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al finalizar la elección";
                }

                return RedirectToAction("EleccionView");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("EleccionView");
            }
        }
    }
}
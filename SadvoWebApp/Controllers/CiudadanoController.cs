using System;
using Microsoft.AspNetCore.Mvc;
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Interfaces;
using Sadvo.Core.Application.Interfaces.AdministradorInterfaces;
using Sadvo.Core.Application.ViewModels;
using Sadvo.Core.Application.ViewModels.Ciudadano;

namespace SADVOWebApplication.Controllers
{
    public class CiudadanoController : Controller
    {
        private readonly IValidateUserSession _session;

        private readonly ICiudadanoService _ciudadanoService;
        public string Mensaje { get; set; } = string.Empty;

        public CiudadanoController(IValidateUserSession session, ICiudadanoService ciudadanoService)
        {
            _session = session;
            _ciudadanoService = ciudadanoService;
        }


        //Ciudadano//
        public async Task<IActionResult> CiudadanoView()
        {

            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Login", action = "Login" });
            }

            var puestos = await _ciudadanoService.GetAllListAsync();

            var lista = puestos.Select(p => new CiudadanoViewModel
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Apellido = p.Apellido,
                Identificacion = p.Identificacion,
                Email = p.Email,
                Estado = p.Estado
            }).ToList();

            var vm = new CiudadanoListViewModel
            {
                Ciudadanos = lista
            };

            return View(vm);
        }


        public ActionResult AddCiudadano()
        {
            var model = new CiudadanoViewModel();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddCiudadano(CiudadanoViewModel model)
        {
            // ✅ AGREGAR LOGS PARA DEBUG
            Console.WriteLine($"Datos recibidos: {model.Nombre}, {model.Apellido}, {model.Identificacion}");

            CiudadanoDto dto = new()
            {
                Id = 0,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                Identificacion = model.Identificacion,
                Email = model.Email,
                Estado = model.Estado
            };

            Console.WriteLine($"DTO creado: {dto.Nombre}, {dto.Apellido}, {dto.Identificacion}");

            // Validación 1: Elección activa
            var validacionEleccion = await _ciudadanoService.ValidacionEleccionActiva();
            if (!validacionEleccion.Exito)
            {
                Console.WriteLine($"Validación elección falló: {validacionEleccion.Mensaje}");
                model.Mensaje = validacionEleccion.Mensaje;
                return View("AddCiudadano", model);
            }

            // Validación 2: Identificacion de ciudadano  
            var validacionNombre = await _ciudadanoService.ValidacionCedulaExistente(dto);
            if (!validacionNombre.Exito)
            {
                Console.WriteLine($"Validación cédula falló: {validacionNombre.Mensaje}");
                model.Mensaje = validacionNombre.Mensaje;
                return View("AddCiudadano", model);
            }

            Console.WriteLine("Pasó todas las validaciones, intentando guardar...");
            var resultado = await _ciudadanoService.AddAsync(dto);
            Console.WriteLine($"Resultado del guardado: {resultado}");

            return RedirectToRoute(new { controller = "Ciudadano", action = "CiudadanoView" });
        }


        public async Task<IActionResult> EditCiudadano(int id)
        {
            ViewBag.IsEdit = true;

            var entity = await _ciudadanoService.GetById(id);

            if (entity == null)
            {
                return RedirectToRoute(new { controller = "Ciudadano", action = "CiudadanoView" });
            }

            else
            {
                CiudadanoViewModel valorActualizado = new() { Id = entity.Id, Nombre = entity.Nombre, Apellido = entity.Apellido, Identificacion = entity.Identificacion, Email = entity.Email, Estado = entity.Estado };

                return View("AddCiudadano", valorActualizado);

            }
        }

        [HttpPost]
        public async Task<ActionResult> EditCiudadano(CiudadanoViewModel model, int id)
        {

            ViewBag.EditMode = true;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = await _ciudadanoService.GetById(id);

            if (entity == null)
            {
                return RedirectToRoute(new { controller = "Ciudadano", action = "CiudadanoView" });
            }

            else
            {

                CiudadanoDto dto = new() { Id = model.Id, Nombre = model.Nombre, Apellido = model.Apellido, Identificacion = model.Identificacion, Email = model.Email, Estado = model.Estado };

                // Validación 1: Elección activa
                var validacionEleccion = await _ciudadanoService.ValidacionEleccionActiva();
                if (!validacionEleccion.Exito)
                {
                    model.Mensaje = validacionEleccion.Mensaje;
                    return View("AddCiudadano", model);
                }

                // Validación 2: Identificacion de ciudadano
                var validacionNombre = await _ciudadanoService.ValidacionCedulaExistente(dto);
                if (!validacionNombre.Exito)
                {
                    model.Mensaje = validacionNombre.Mensaje;
                    return View("AddCiudadano", model);
                }

                var resultado = await _ciudadanoService.UpdateAsync(id, dto);

                return RedirectToRoute(new { controller = "Ciudadano", action = "CiudadanoView" });

            }
        }


        // GET: Mostrar pantalla de confirmación
        [HttpGet]
        public async Task<ActionResult> ConfirmarCambioEstado(int id)
        {
            var entity = await _ciudadanoService.GetById(id);
            if (entity == null)
            {
                return RedirectToRoute(new { controller = "Ciudadano", action = "CiudadanoView" });
            }

            var viewModel = new ConfirmarEstadoViewModel
            {
                UsuarioId = id,
                EstadoActual = entity.Estado,
                NuevoEstado = entity.Estado == Sadvo.Core.Domain.Enums.Actividad.Activo ? Sadvo.Core.Domain.Enums.Actividad.Inactivo : Sadvo.Core.Domain.Enums.Actividad.Activo
            };

            return View("ConfirmarCambioEstado", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstadoUsuario(ConfirmarEstadoViewModel model, int id)
        {
            // Ensure the model is properly initialized
            if (model == null)
            {
                model = new ConfirmarEstadoViewModel();
            }

            var entity = await _ciudadanoService.GetById(id);
            if (entity == null)
            {
                return RedirectToRoute(new { controller = "Ciudadano", action = "CiudadanoView" });
            }

            var nuevoEstado = entity.Estado == Sadvo.Core.Domain.Enums.Actividad.Activo ? Sadvo.Core.Domain.Enums.Actividad.Inactivo : Sadvo.Core.Domain.Enums.Actividad.Activo;
            model.EstadoActual = entity.Estado;
            model.NuevoEstado = nuevoEstado;
            model.UsuarioId = id;

            // Validación 1: Elección activa
            var validacionEleccion = await _ciudadanoService.ValidacionEleccionActiva();
            if (!validacionEleccion.Exito)
            {
                model.Mensaje = validacionEleccion.Mensaje;
                return View("ConfirmarCambioEstado", model);
            }

            CiudadanoDto estadoDto = new()
            {
                Id = id,
                Estado = nuevoEstado
            };

            await _ciudadanoService.CambiarEstadoAsync(id, estadoDto);

            return RedirectToRoute(new { controller = "Ciudadano", action = "CiudadanoView" });
        }
    }
}

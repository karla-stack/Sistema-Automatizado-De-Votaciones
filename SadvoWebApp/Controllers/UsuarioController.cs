using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Interfaces;
using Sadvo.Core.Application.Interfaces.AdministradorInterfaces;
using Sadvo.Core.Application.ViewModels;
using Sadvo.Core.Application.ViewModels.Usuarios;
using Sadvo.Core.Domain.Entities.Administrador;

namespace SADVOWebApplication.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IValidateUserSession _session;

        private readonly IUsuarioService _usuarioService;

        public string Mensaje { get; set; } = string.Empty;

        public UsuarioController(IValidateUserSession session, IUsuarioService usuarioService)
        {
            _session = session;
            _usuarioService = usuarioService;
        }

        //Usuarios//
        public async Task<ActionResult> UsuarioView()
        {
            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Login", action = "Login" });
            }

            var usuarios = await _usuarioService.GetAllListAsync();

            var lista = usuarios.Select(p => new UserMantenimientoViewModel
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Apellido = p.Apellido,
                Email = p.Email,
                NombreUsuario = p.NombreUsuario,
                Contrasena = p.Contrasena,
                Estado = p.Estado,
                Rol = p.Rol

            }).ToList();

            var vm = new UserListViewModel
            {
                Usuarios = lista
            };
            return View(vm);
        }


        public ActionResult AddUsuario()
        {
            var model = new UserMantenimientoViewModel();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddUsuario(UserMantenimientoViewModel model)
        {
            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Usuario", action = "Login" });
            }

            UserMantenimientoViewModel dto = new()
            {
                Id = 0,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                Email = model.Email,
                NombreUsuario = model.NombreUsuario,
                Contrasena = model.Contrasena,
                Estado = model.Estado,
                Rol = model.Rol
            };


            UsuarioDto validacion = new()
            {

                Id = dto.Id,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                NombreUsuario = dto.NombreUsuario,
                Contrasena = dto.Contrasena,
                Estado = dto.Estado,
                Rol = dto.Rol
            };

            // Validación 1: Elección activa
            var validacionEleccion = await _usuarioService.ValidacionEleccionActiva();
            if (!validacionEleccion.Exito)
            {
                model.Mensaje = validacionEleccion.Mensaje;
                return View("AddUsuario", model);
            }

            // Validación 2: Nombre de usuario existente
            var validacionNombre = await _usuarioService.ValidacionNombreExistente(validacion);
            if (!validacionNombre.Exito)
            {
                model.Mensaje = validacionNombre.Mensaje;
                return View("AddUsuario", model);
            }

            var resultado = await _usuarioService.AddAsync(validacion);

            if (resultado == null || resultado == false)
            {
                model.Mensaje = "Ocurrió un error al agregar el usuario.";
                return View("AddUsuario", model);
            }

            // Todo salió bien
            return RedirectToAction("UsuarioView");
        }




        public async Task<IActionResult> EditUsuario(int id)
        {
            ViewBag.IsEdit = true;

            var entity = await _usuarioService.GetById(id);

            if (entity == null)
            {
                return RedirectToRoute(new { controller = "Usuario", action = "UsuarioView" });
            }

            else
            {
                UserMantenimientoViewModel valorActualizado = new()
                {
                    Id = entity.Id,
                    Nombre = entity.Nombre,
                    Apellido = entity.Apellido,
                    Email = entity.Email,
                    NombreUsuario = entity.NombreUsuario,
                    Contrasena = entity.Contrasena,
                    Estado = entity.Estado,
                    Rol = entity.Rol
                };


                return View("AddUsuario", valorActualizado);

            }
        }
        [HttpPost]
        public async Task<ActionResult> EditUsuario(UserMantenimientoViewModel model, int id)
        {

            ViewBag.IsEdit = true;


            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = await _usuarioService.GetById(id);

            if (entity == null)
            {
                return RedirectToRoute(new { controller = "Usuario", action = "UsuarioView" });
            }

            else
            {

                UsuarioDto dto = new() { Id = model.Id, Nombre = model.Nombre, Apellido = model.Apellido, Email = model.Email, NombreUsuario = model.NombreUsuario, Contrasena = model.Contrasena, Estado = model.Estado, Rol = model.Rol };

                // Validación 1: Elección activa
                var validacionEleccion = await _usuarioService.ValidacionEleccionActiva();
                if (!validacionEleccion.Exito)
                {
                    model.Mensaje = validacionEleccion.Mensaje;
                    return View("AddUsuario", model);
                }

                // Validación 2: Nombre de usuario existente
                var validacionNombre = await _usuarioService.ValidacionNombreExistente(dto);
                if (!validacionNombre.Exito)
                {
                    model.Mensaje = validacionNombre.Mensaje;
                    return View("AddUsuario", model);
                }


                var resultado = await _usuarioService.UpdateAsync(id, dto);

                return RedirectToRoute(new { controller = "Usuario", action = "UsuarioView" });

            }
        }
        // GET: Mostrar pantalla de confirmación
        [HttpGet]
        public async Task<ActionResult> ConfirmarCambioEstado(int id)
        {
            var entity = await _usuarioService.GetById(id);
            if (entity == null)
            {
                return RedirectToRoute(new { controller = "Usuario", action = "UsuarioView" });
            }

            var viewModel = new ConfirmarEstadoViewModel
            {
                UsuarioId = id,
                NombreUsuario = entity.NombreUsuario, // Asume que tienes esta propiedad
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

            var entity = await _usuarioService.GetById(id);
            if (entity == null)
            {
                return RedirectToRoute(new { controller = "Usuario", action = "UsuarioView" });
            }

            var nuevoEstado = entity.Estado == Sadvo.Core.Domain.Enums.Actividad.Activo ? Sadvo.Core.Domain.Enums.Actividad.Inactivo : Sadvo.Core.Domain.Enums.Actividad.Activo;
            model.EstadoActual = entity.Estado;
            model.NuevoEstado = nuevoEstado;
            model.UsuarioId = id;
            model.NombreUsuario = entity.NombreUsuario;

            // Validación 1: Elección activa
            var validacionEleccion = await _usuarioService.ValidacionEleccionActiva();
            if (!validacionEleccion.Exito)
            {
                model.Mensaje = validacionEleccion.Mensaje;
                // Return to the same confirmation view with error message
                return View("ConfirmarCambioEstado", model);
            }

            UsuarioDto estadoDto = new()
            {
                Id = id,
                Estado = nuevoEstado
            };

            await _usuarioService.CambiarEstadoAsync(id, estadoDto);

            return RedirectToRoute(new { controller = "Usuario", action = "UsuarioView" });
        }

    }
}

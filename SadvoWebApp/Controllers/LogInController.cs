using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Interfaces;
using Sadvo.Core.Application.Interfaces.AdministradorInterfaces;
using Sadvo.Core.Application.ViewModels.Usuarios;
using Sadvo.Core.Application.Helpers;
using Sadvo.Core.Domain.Enums;

namespace SADVOWebApplication.Controllers
{
    public class LogInController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IValidateUserSession _validateUserSession;

        public LogInController(IUsuarioService usuario, IValidateUserSession validateUserSession)
        {
            _usuarioService = usuario;
            _validateUserSession = validateUserSession;
        }

        public IActionResult Login(string username, string contrasena)
        {
            if (_validateUserSession.HasUserSession())
            {
                UserViewModel? userSession = _validateUserSession.GetUserSession();

                if (userSession != null)
                {
                    return userSession.Rol switch
                    {
                        Sadvo.Core.Domain.Enums.RolUsuario.Administrador => RedirectToRoute(new { controller = "Menu", action = "Menu" }),
                        Sadvo.Core.Domain.Enums.RolUsuario.Dirigente => RedirectToRoute(new { controller = "Dirigente", action = "Menu" }),
                        _ => RedirectToRoute(new { controller = "Login", action = "Login" }),
                    };
                }
            }

            return View(new UsuarioLogInViewModel() { NombreUsuario = "", Contrasena = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Login(UsuarioLogInViewModel vm)
        {
            if (_validateUserSession.HasUserSession())
            {
                UserViewModel? userSession = _validateUserSession.GetUserSession();

                if (userSession != null)
                {
                    return userSession.Rol switch
                    {
                        Sadvo.Core.Domain.Enums.RolUsuario.Administrador => RedirectToRoute(new { controller = "Menu", action = "Menu" }),
                        Sadvo.Core.Domain.Enums.RolUsuario.Dirigente => RedirectToRoute(new { controller = "Dirigente", action = "Menu" }),
                        _ => RedirectToRoute(new { controller = "Login", action = "Login" }),
                    };
                }
            }

            // Validaciones de campos requeridos
            if (string.IsNullOrEmpty(vm.NombreUsuario))
            {
                vm.Mensaje = "El usuario es requerido";
                return View(vm);
            }

            if (string.IsNullOrEmpty(vm.Contrasena))
            {
                vm.Mensaje = "La contraseña es requerida";
                return View(vm);
            }

            if (!ModelState.IsValid)
            {
                vm.Contrasena = "";
                return View(vm);
            }

            // Intentar hacer login
            UsuarioDto? login = await _usuarioService.LogInAsync(new LoginDto() { NombreUsuario = vm.NombreUsuario, contrasena = vm.Contrasena });

            if (login != null)
            {
                // Validar si el usuario está activo
                if (login.Estado != Actividad.Activo) // Asumiendo que existe una propiedad EstaActivo en UsuarioDto
                {
                    vm.Mensaje = "Su cuenta está inactiva. Por favor, póngase en contacto con un administrador.";
                    vm.Contrasena = "";
                    return View(vm);
                }

                var usuarioCompleto = await _usuarioService.GetByUsernameWithAsignacionAsync(login.NombreUsuario);

                // Validar si es dirigente y tiene partido político asignado
                if (login.Rol == Sadvo.Core.Domain.Enums.RolUsuario.Dirigente)
                {
                    if (usuarioCompleto?.AsignacionDirigente?.PartidoPoliticoId == null ||
                        usuarioCompleto.AsignacionDirigente.PartidoPoliticoId == 0)
                    {
                        vm.Mensaje = "No tiene un partido político asignado, por lo tanto no puede iniciar sesión. Por favor, póngase en contacto con un administrador.";
                        vm.Contrasena = "";
                        return View(vm);
                    }
                }

                // Crear sesión de usuario
                UserViewModel view = new()
                {
                    Id = login.Id,
                    Nombre = login.Nombre,
                    Apellido = login.Apellido,
                    NombreUsuario = login.NombreUsuario,
                    Contrasena = login.Contrasena,
                    Email = login.Email,
                    Rol = login.Rol,
                    PartidoPoliticoId = usuarioCompleto?.AsignacionDirigente?.PartidoPoliticoId
                };

                HttpContext.Session.Set("User", view);

                // Redireccionar según el rol
                if (view.Rol == Sadvo.Core.Domain.Enums.RolUsuario.Administrador)
                {
                    return RedirectToRoute(new { controller = "Menu", action = "Menu" });
                }
                return RedirectToRoute(new { controller = "Dirigente", action = "Menu" });
            }
            else
            {
                ModelState.AddModelError("userValidation", "Acceso incorrecto");
                vm.Mensaje = "Usuario o contraseña incorrectos.";
            }

            vm.Contrasena = "";
            return View(vm);
        }

        public IActionResult Logout(string username, string contrasena)
        {
            HttpContext.Session.Remove("User");
            HttpContext.Session.Clear();
            return RedirectToRoute(new { controller = "Home", action = "Index" });
        }

        [HttpPost]
        public async Task<IActionResult> Register(UsuarioLogInViewModel vm)
        {
            var resultado = await _usuarioService.CrearAdmin();

            if (resultado != false)
            {
                vm.Mensaje = "Usuario: SuperAdmin Contraseña: admin123";
                return RedirectToAction("Login");
            }

            return View("Login", vm.Mensaje);
        }
    }
}
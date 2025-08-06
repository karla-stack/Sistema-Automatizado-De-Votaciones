using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Sadvo.Core.Application.Interfaces;
using Sadvo.Core.Application.Interfaces.AdministradorInterfaces;
using Sadvo.Core.Application.Interfaces.DirigenteInterfaces;
using Sadvo.Core.Application.ViewModels.Elector;
using Sadvo.Core.Domain.Entities.Elector;
using Sadvo.Core.Domain.Interfaces;

namespace SADVO.Web.Controllers
{
    public class ElectorController : Controller
    {
        private readonly IOcrService _ocrService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICiudadanoService _ciudadanoService;
        private readonly IEleccionService _eleccionService;
        private readonly ICandidatoService _candidatoService;
        private readonly IPuestoElectivoService _puestoElectivoService;
        private readonly IVotoRepository _votoRepository;

        public ElectorController(
            IOcrService ocrService,
            IHttpContextAccessor httpContextAccessor,
            ICiudadanoService ciudadanoService,
            IEleccionService eleccionService,
            ICandidatoService candidatoService,
            IPuestoElectivoService puestoElectivoService,
            IVotoRepository votoRepository)
        {
            _ocrService = ocrService;
            _httpContextAccessor = httpContextAccessor;
            _ciudadanoService = ciudadanoService;
            _eleccionService = eleccionService;
            _candidatoService = candidatoService;
            _puestoElectivoService = puestoElectivoService;
            _votoRepository = votoRepository;
        }

        // =================================================================
        // PROCESAMIENTO DEL FORMULARIO INICIAL
        // =================================================================
        [HttpPost]
        public async Task<IActionResult> Index(string citizenIdCard)
        {
            if (string.IsNullOrWhiteSpace(citizenIdCard))
            {
                TempData["ErrorMessage"] = "Debe ingresar su número de documento de identidad.";
                return RedirectToAction("Index", "Home");
            }

            citizenIdCard = citizenIdCard.Replace("-", "").Replace(" ", "").Trim();

            if (!Regex.IsMatch(citizenIdCard, @"^\d{11}$"))
            {
                TempData["ErrorMessage"] = "El número de documento debe tener 11 dígitos.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // 1. Verificar elección activa
                var hasActiveElection = await _eleccionService.HayEleccionActiva();
                if (!hasActiveElection)
                {
                    TempData["ErrorMessage"] = "No hay ningún proceso electoral en estos momentos.";
                    return RedirectToAction("Index", "Home");
                }

                // 2. Verificar ciudadano existe
                var ciudadanos = await _ciudadanoService.GetAllListAsync();
                var ciudadano = ciudadanos.FirstOrDefault(c => c.Identificacion == citizenIdCard);

                if (ciudadano == null)
                {
                    TempData["ErrorMessage"] = "Su documento de identidad no se encuentra registrado en el sistema.";
                    return RedirectToAction("Index", "Home");
                }

                // 3. Verificar ciudadano activo
                if (ciudadano.Estado != Sadvo.Core.Domain.Enums.Actividad.Activo)
                {
                    TempData["ErrorMessage"] = "Su cuenta está inactiva. Contacte al administrador del sistema.";
                    return RedirectToAction("Index", "Home");
                }

                // 4. Verificar si ya votó
                var hasVoted = await _votoRepository.HasVotedInActiveElectionAsync(ciudadano.Id);
                if (hasVoted)
                {
                    TempData["ErrorMessage"] = "Ya ha ejercido su derecho al voto en esta elección.";
                    return RedirectToAction("Index", "Home");
                }

                // Guardar en sesión
                HttpContext.Session.SetString("citizenIdCard", citizenIdCard);
                HttpContext.Session.SetString("ciudadanoId", ciudadano.Id.ToString());
                return RedirectToAction("ValidateOcr");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al validar sus datos. Intente nuevamente.";
                return RedirectToAction("Index", "Home");
            }
        }

        // =================================================================
        // VALIDACIÓN OCR
        // =================================================================
        public IActionResult ValidateOcr()
        {
            var citizenIdCard = HttpContext.Session.GetString("citizenIdCard");
            if (string.IsNullOrEmpty(citizenIdCard))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ValidateOcr(IFormFile idCardFile)
        {
            var citizenIdCardFromSession = HttpContext.Session.GetString("citizenIdCard");

            if (idCardFile == null)
            {
                ModelState.AddModelError("", "Debe seleccionar una imagen de su cédula.");
                return View();
            }

            try
            {
                await using var memoryStream = new MemoryStream();
                await idCardFile.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                var idFromImage = await _ocrService.ExtractIdNumberFromImageAsync(imageBytes);

                if (idFromImage != citizenIdCardFromSession)
                {
                    ModelState.AddModelError("", "El número de documento en la imagen no coincide con el ingresado. Por favor, intente de nuevo.");
                    return View();
                }

                HttpContext.Session.SetString("ocrValidated", "true");
                return RedirectToAction("SelectPuesto");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al procesar la imagen. Intente nuevamente.");
                return View();
            }
        }

        // =================================================================
        // SELECCIÓN DE PUESTOS
        // =================================================================
        public async Task<IActionResult> SelectPuesto()
        {
            var ocrValidated = HttpContext.Session.GetString("ocrValidated");
            if (string.IsNullOrEmpty(ocrValidated) || ocrValidated != "true")
            {
                return RedirectToAction("ValidateOcr");
            }

            var ciudadanoIdStr = HttpContext.Session.GetString("ciudadanoId");
            if (string.IsNullOrEmpty(ciudadanoIdStr) || !int.TryParse(ciudadanoIdStr, out int ciudadanoId))
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var allPuestos = await _puestoElectivoService.GetAllListAsync();
                var activePuestos = allPuestos.Where(p => p.Estado == Sadvo.Core.Domain.Enums.Actividad.Activo);
                var votedPuestos = await _votoRepository.GetVotedPuestosByElectorAsync(ciudadanoId);

                var model = new List<SelectPuestoViewModel>();

                foreach (var puesto in activePuestos)
                {
                    var candidatosPuesto = await _votoRepository.GetCandidatosByPuestoAsync(puesto.Id);
                    var hasVoted = votedPuestos.Any(v => v.PuestoElectivoId == puesto.Id);

                    model.Add(new SelectPuestoViewModel
                    {
                        PuestoId = puesto.Id,
                        PuestoNombre = puesto.Nombre,
                        Descripcion = puesto.Descripcion,
                        TotalCandidatos = candidatosPuesto.Count(),
                        HasVoted = hasVoted,
                        IsActive = true
                    });
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar los puestos disponibles.";
                return View(new List<SelectPuestoViewModel>());
            }
        }

        // =================================================================
        // VOTACIÓN POR PUESTO
        // =================================================================
        public async Task<IActionResult> VoteForPuesto(int puestoId)
        {
            var ocrValidated = HttpContext.Session.GetString("ocrValidated");
            if (string.IsNullOrEmpty(ocrValidated) || ocrValidated != "true")
            {
                return RedirectToAction("ValidateOcr");
            }

            var ciudadanoIdStr = HttpContext.Session.GetString("ciudadanoId");
            if (string.IsNullOrEmpty(ciudadanoIdStr) || !int.TryParse(ciudadanoIdStr, out int ciudadanoId))
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var puesto = await _puestoElectivoService.GetById(puestoId);
                if (puesto == null || puesto.Estado != Sadvo.Core.Domain.Enums.Actividad.Activo)
                {
                    TempData["ErrorMessage"] = "El puesto seleccionado no está disponible.";
                    return RedirectToAction("SelectPuesto");
                }

                var hasVoted = await _votoRepository.HasVotedForPuestoAsync(ciudadanoId, puestoId);
                if (hasVoted)
                {
                    TempData["InfoMessage"] = $"Ya ha votado por el puesto de {puesto.Nombre}.";
                    return RedirectToAction("SelectPuesto");
                }

                var candidatosPuesto = await _votoRepository.GetCandidatosByPuestoAsync(puestoId);

                var model = new VoteForPuestoViewModel
                {
                    PuestoId = puestoId,
                    PuestoNombre = puesto.Nombre,
                    PuestoDescripcion = puesto.Descripcion,
                    Candidatos = candidatosPuesto.Select(c => new CandidatoVotingViewModel
                    {
                        CandidatoId = c.Id,
                        Nombre = c.Nombre,
                        Apellido = c.Apellido,
                        Foto = c.Foto ?? "",
                        PartidoNombre = c.PartidoPolitico?.Nombre ?? "Sin partido",
                        PartidoSiglas = c.PartidoPolitico?.Siglas ?? "",
                        PartidoLogo = c.PartidoPolitico?.Logo ?? "",
                        PartidoId = c.PartidoPoliticoId
                    }).ToList()
                };

                HttpContext.Session.SetString("currentPuestoId", puestoId.ToString());
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar los candidatos.";
                return RedirectToAction("SelectPuesto");
            }
        }

        [HttpPost]
        public async Task<IActionResult> VoteForPuesto(VoteForPuestoViewModel model)
        {
            var ocrValidated = HttpContext.Session.GetString("ocrValidated");
            if (string.IsNullOrEmpty(ocrValidated) || ocrValidated != "true")
            {
                return RedirectToAction("ValidateOcr");
            }

            var ciudadanoIdStr = HttpContext.Session.GetString("ciudadanoId");
            if (string.IsNullOrEmpty(ciudadanoIdStr) || !int.TryParse(ciudadanoIdStr, out int ciudadanoId))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid || model.SelectedCandidatoId <= 0)
            {
                ModelState.AddModelError("", "Debe seleccionar un candidato antes de votar.");
                return await VoteForPuesto(model.PuestoId);
            }

            try
            {
                var activeElection = await _eleccionService.GetEleccionActiva();
                if (activeElection == null)
                {
                    TempData["ErrorMessage"] = "No hay proceso electoral activo.";
                    return RedirectToAction("SelectPuesto");
                }

                var candidato = await _candidatoService.GetById(model.SelectedCandidatoId);
                if (candidato == null || candidato.Estado != Sadvo.Core.Domain.Enums.Actividad.Activo)
                {
                    TempData["ErrorMessage"] = "El candidato seleccionado no es válido.";
                    return RedirectToAction("SelectPuesto");
                }

                var voto = new Voto
                {
                    CiudadanoId = ciudadanoId,
                    EleccionId = activeElection.Id,
                    PuestoElectivoId = model.PuestoId,
                    CandidatoId = model.SelectedCandidatoId,
                    PartidoPoliticoId = candidato.PartidoPoliticoId,
                    FechaVoto = DateTime.UtcNow
                };

                await _votoRepository.CreateVoteAsync(voto);

                HttpContext.Session.Remove("currentPuestoId");
                TempData["SuccessMessage"] = $"Su voto por {candidato.Nombre} {candidato.Apellido} ha sido registrado exitosamente.";

                var hasCompletedAll = await HasCompletedAllVotes(ciudadanoId);
                if (hasCompletedAll)
                {
                    return RedirectToAction("VotingComplete");
                }

                return RedirectToAction("SelectPuesto");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al procesar su voto.";
                return RedirectToAction("SelectPuesto");
            }
        }

        // =================================================================
        // FINALIZACIÓN
        // =================================================================
        public async Task<IActionResult> FinishVoting()
        {
            var ciudadanoIdStr = HttpContext.Session.GetString("ciudadanoId");
            if (string.IsNullOrEmpty(ciudadanoIdStr) || !int.TryParse(ciudadanoIdStr, out int ciudadanoId))
            {
                return RedirectToAction("Index", "Home");
            }

            var hasCompletedAll = await HasCompletedAllVotes(ciudadanoId);
            if (!hasCompletedAll)
            {
                var missingPuestos = await GetMissingPuestos(ciudadanoId);
                TempData["WarningMessage"] = $"Aún debe votar por los siguientes puestos: {string.Join(", ", missingPuestos)}";
                return RedirectToAction("SelectPuesto");
            }

            return RedirectToAction("VotingComplete");
        }

        public IActionResult VotingComplete()
        {
            HttpContext.Session.Clear();

            var model = new VotingCompleteViewModel
            {
                CompletionMessage = "¡Felicidades! Ha completado exitosamente su proceso de votación.",
                HasError = false,
                CompletionTime = DateTime.Now
            };

            return View(model);
        }

        // =================================================================
        // MÉTODOS AUXILIARES
        // =================================================================
        private async Task<bool> HasCompletedAllVotes(int ciudadanoId)
        {
            try
            {
                var votedCount = await _votoRepository.GetVotedPuestosCountAsync(ciudadanoId);
                var allPuestos = await _puestoElectivoService.GetAllListAsync();
                var totalActive = allPuestos.Count(p => p.Estado == Sadvo.Core.Domain.Enums.Actividad.Activo);

                return votedCount >= totalActive;
            }
            catch
            {
                return false;
            }
        }

        private async Task<List<string>> GetMissingPuestos(int ciudadanoId)
        {
            try
            {
                var allPuestos = await _puestoElectivoService.GetAllListAsync();
                var activePuestos = allPuestos.Where(p => p.Estado == Sadvo.Core.Domain.Enums.Actividad.Activo);
                var votedPuestos = await _votoRepository.GetVotedPuestosByElectorAsync(ciudadanoId);
                var votedPuestoIds = votedPuestos.Select(v => v.PuestoElectivoId).ToHashSet();

                return activePuestos
                    .Where(p => !votedPuestoIds.Contains(p.Id))
                    .Select(p => p.Nombre)
                    .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}
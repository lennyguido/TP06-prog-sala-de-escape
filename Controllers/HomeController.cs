using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Tp06.Models;

namespace Tp06.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private BD bd = new BD();

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    // ================== IDENTIFICARSE ==================
    public IActionResult Identificarse()
    {
        return View();
    }

    [HttpPost]
    public IActionResult IniciarPartida(string nombreParticipante)
    {
        if (string.IsNullOrWhiteSpace(nombreParticipante))
        {
            ViewBag.Error = "El nombre no puede estar vacío.";
            return View("Identificarse");
        }

        int partidaId = bd.CrearPartida(nombreParticipante.Trim());
        var primeraSala = bd.ObtenerPrimeraSala();
        bd.CrearProgreso(partidaId, primeraSala.SalaId);

        HttpContext.Session.SetInt32("PartidaId", partidaId);
        HttpContext.Session.SetString("NombreParticipante", nombreParticipante.Trim());

        return RedirectToAction("Sala1");
    }

    // ================== SALA 1 ==================
    public IActionResult Sala1(string respuesta)
    {
        int? partidaId = HttpContext.Session.GetInt32("PartidaId");
        if (partidaId == null) return RedirectToAction("Identificarse");

        var partida = bd.ObtenerPartida(partidaId.Value);
        if (partida == null || partida.Estado != "Activa")
            return RedirectToAction("Index");

        if (!bd.PuedeAccederASala(partidaId.Value, 1))
            return RedirectToAction("Index");

        var sala = bd.ObtenerSala(1);

        if (!string.IsNullOrEmpty(respuesta))
        {
            var progreso = bd.ObtenerProgreso(partidaId.Value, sala.SalaId);
            bool esCorrecta = respuesta.Trim().Equals(sala.RespuestaEsperada.Trim(), StringComparison.OrdinalIgnoreCase);

            if (esCorrecta)
            {
                bd.CompletarSala(partidaId.Value, sala.SalaId);
                var siguiente = bd.ObtenerSalaSiguiente(sala.Orden);
                bd.CrearProgreso(partidaId.Value, siguiente.SalaId);
                return RedirectToAction("Sala2");
            }
            else
            {
                if (progreso.IntentosUsados + 1 >= sala.LimiteIntentos)
                {
                    bd.VencerPartida(partidaId.Value, 0);
                    return RedirectToAction("Incorrecto");
                }
                bd.SumarIntento(partidaId.Value, sala.SalaId);
                return RedirectToAction("Incorrecto");
            }
        }

        ViewBag.Sala = sala;
        return View();
    }

    // ================== SALA 2 ==================
    public IActionResult Sala2(string respuesta)
    {
        int? partidaId = HttpContext.Session.GetInt32("PartidaId");
        if (partidaId == null) return RedirectToAction("Identificarse");

        var partida = bd.ObtenerPartida(partidaId.Value);
        if (partida == null || partida.Estado != "Activa")
            return RedirectToAction("Index");

        if (!bd.PuedeAccederASala(partidaId.Value, 2))
            return RedirectToAction("Sala1");

        var sala = bd.ObtenerSala(2);

        if (!string.IsNullOrEmpty(respuesta))
        {
            var progreso = bd.ObtenerProgreso(partidaId.Value, sala.SalaId);
            bool esCorrecta = respuesta.Trim().Equals(sala.RespuestaEsperada.Trim(), StringComparison.OrdinalIgnoreCase);

            if (esCorrecta)
            {
                bd.CompletarSala(partidaId.Value, sala.SalaId);
                var siguiente = bd.ObtenerSalaSiguiente(sala.Orden);
                bd.CrearProgreso(partidaId.Value, siguiente.SalaId);
                return RedirectToAction("Sala3");
            }
            else
            {
                if (progreso.IntentosUsados + 1 >= sala.LimiteIntentos)
                {
                    bd.VencerPartida(partidaId.Value, 0);
                    return RedirectToAction("Incorrecto");
                }
                bd.SumarIntento(partidaId.Value, sala.SalaId);
                return RedirectToAction("Incorrecto");
            }
        }

        ViewBag.Sala = sala;
        return View();
    }

    // ================== SALA 3 ==================
    public IActionResult Sala3(string respuesta)
    {
        int? partidaId = HttpContext.Session.GetInt32("PartidaId");
        if (partidaId == null) return RedirectToAction("Identificarse");

        var partida = bd.ObtenerPartida(partidaId.Value);
        if (partida == null || partida.Estado != "Activa")
            return RedirectToAction("Index");

        if (!bd.PuedeAccederASala(partidaId.Value, 3))
            return RedirectToAction("Sala1");

        var sala = bd.ObtenerSala(3);

        if (!string.IsNullOrEmpty(respuesta))
        {
            var progreso = bd.ObtenerProgreso(partidaId.Value, sala.SalaId);
            bool esCorrecta = respuesta.Trim().Equals(sala.RespuestaEsperada.Trim(), StringComparison.OrdinalIgnoreCase);

            if (esCorrecta)
            {
                bd.CompletarSala(partidaId.Value, sala.SalaId);
                var siguiente = bd.ObtenerSalaSiguiente(sala.Orden);
                bd.CrearProgreso(partidaId.Value, siguiente.SalaId);
                return RedirectToAction("Sala4");
            }
            else
            {
                if (progreso.IntentosUsados + 1 >= sala.LimiteIntentos)
                {
                    bd.VencerPartida(partidaId.Value, 0);
                    return RedirectToAction("Incorrecto");
                }
                bd.SumarIntento(partidaId.Value, sala.SalaId);
                return RedirectToAction("Incorrecto");
            }
        }

        ViewBag.Sala = sala;
        return View();
    }

    // ================== SALA 4 ==================
    public IActionResult Sala4(string respuesta)
    {
        int? partidaId = HttpContext.Session.GetInt32("PartidaId");
        if (partidaId == null) return RedirectToAction("Identificarse");

        var partida = bd.ObtenerPartida(partidaId.Value);
        if (partida == null || partida.Estado != "Activa")
            return RedirectToAction("Index");

        if (!bd.PuedeAccederASala(partidaId.Value, 4))
            return RedirectToAction("Sala1");

        var sala = bd.ObtenerSala(4);

        if (!string.IsNullOrEmpty(respuesta))
        {
            var progreso = bd.ObtenerProgreso(partidaId.Value, sala.SalaId);
            bool esCorrecta = respuesta.Trim().Equals(sala.RespuestaEsperada.Trim(), StringComparison.OrdinalIgnoreCase);

            if (esCorrecta)
            {
                bd.CompletarSala(partidaId.Value, sala.SalaId);
                bd.FinalizarPartida(partidaId.Value, 0); // última sala -> gana la partida
                return RedirectToAction("Victoria");
            }
            else
            {
                if (progreso.IntentosUsados + 1 >= sala.LimiteIntentos)
                {
                    bd.VencerPartida(partidaId.Value, 0);
                    return RedirectToAction("Incorrecto");
                }
                bd.SumarIntento(partidaId.Value, sala.SalaId);
                return RedirectToAction("Incorrecto");
            }
        }

        ViewBag.Sala = sala;
        return View();
    }

    public IActionResult Victoria()
    {
        return View();
    }

    public IActionResult Incorrecto()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
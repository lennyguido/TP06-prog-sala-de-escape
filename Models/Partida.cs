namespace Tp06.Models;

public class Partida
{
    public int PartidaId { get; set; }
    public string NombreParticipante { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string Estado { get; set; }
    public int TiempoConsumido { get; set; }
}
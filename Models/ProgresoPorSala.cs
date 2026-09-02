namespace TP06.Models;

public class ProgresoPorSala
{
    public int ProgresoId { get; set; }
    public int PartidaId { get; set; }
    public int SalaId { get; set; }
    public string Estado { get; set; }
    public int IntentosUsados { get; set; }
    public int PistasSolicitadas { get; set; }
    public DateTime FechaUltimaAccion { get; set; }
}
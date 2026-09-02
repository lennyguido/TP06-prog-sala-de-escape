namespace Tp06.Models;

public class Sala
{
    public int SalaId { get; set; }
    public string Nombre { get; set; }
    public string Narrativa { get; set; }
    public string Acertijo { get; set; }
    public string RespuestaEsperada { get; set; }
    public int Orden { get; set; }
    public int LimiteIntentos { get; set; }
    public int LimitePistas { get; set; }
}
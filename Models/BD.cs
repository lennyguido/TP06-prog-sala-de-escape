using Dapper;
using Microsoft.Data.SqlClient;
using Tp06.Models;

namespace Tp06.Models;

public class BD
{
    private string _connectionString = @"Server=localhost;DataBase=TP06Prog;Integrated Security=True;TrustServerCertificate=True;";

    // =========================================================
    //  PARTIDAS
    // =========================================================

    // CREAR UNA PARTIDA Y DEVOLVER SU ID (así no hace falta un segundo query)
    public int CrearPartida(string nombreParticipante)
    {
        string query = @"INSERT INTO Partidas
                        (NombreParticipante, FechaInicio, FechaFin, Estado, TiempoConsumido)
                        VALUES
                        (@NombreParticipante, @FechaInicio, NULL, @Estado, NULL);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.QuerySingle<int>(query, new
            {
                NombreParticipante = nombreParticipante,
                FechaInicio = DateTime.Now,
                Estado = "Activa"
            });
        }
    }

    // OBTENER UNA PARTIDA POR SU ID
    public Partida? ObtenerPartida(int partidaId)
    {
        string query = @"SELECT * FROM Partidas WHERE PartidaId = @PartidaId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.QueryFirstOrDefault<Partida>(query, new { PartidaId = partidaId });
        }
    }

    // OBTENER TODAS LAS PARTIDAS DE UN JUGADOR (para el listado "Mis partidas")
    public List<Partida> ObtenerPartidasPorJugador(string nombreParticipante)
    {
        string query = @"SELECT * FROM Partidas
                         WHERE NombreParticipante = @NombreParticipante
                         ORDER BY PartidaId DESC";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.Query<Partida>(query, new { NombreParticipante = nombreParticipante }).ToList();
        }
    }

    // GUARDAR TIEMPO CONSUMIDO (sin cambiar el estado)
    public void ActualizarTiempo(int partidaId, int tiempoConsumido)
    {
        string query = @"UPDATE Partidas SET TiempoConsumido = @TiempoConsumido
                         WHERE PartidaId = @PartidaId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new { TiempoConsumido = tiempoConsumido, PartidaId = partidaId });
        }
    }

    // FINALIZAR PARTIDA GANADA
    public void FinalizarPartida(int partidaId, int tiempoConsumido)
    {
        string query = @"UPDATE Partidas
                         SET Estado = 'Completada', FechaFin = @FechaFin, TiempoConsumido = @TiempoConsumido
                         WHERE PartidaId = @PartidaId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new { FechaFin = DateTime.Now, TiempoConsumido = tiempoConsumido, PartidaId = partidaId });
        }
    }

    // MARCAR PARTIDA COMO VENCIDA (se acabó el tiempo o los intentos)
    public void VencerPartida(int partidaId, int tiempoConsumido)
    {
        string query = @"UPDATE Partidas
                         SET Estado = 'Vencida', FechaFin = @FechaFin, TiempoConsumido = @TiempoConsumido
                         WHERE PartidaId = @PartidaId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new { FechaFin = DateTime.Now, TiempoConsumido = tiempoConsumido, PartidaId = partidaId });
        }
    }

    // =========================================================
    //  SALAS
    // =========================================================

    // OBTENER UNA SALA POR ID
    public Sala? ObtenerSala(int salaId)
    {
        string query = @"SELECT * FROM Salas WHERE SalaId = @SalaId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.QueryFirstOrDefault<Sala>(query, new { SalaId = salaId });
        }
    }

    // OBTENER LA PRIMERA SALA DEL JUEGO (Orden mínimo)
    public Sala? ObtenerPrimeraSala()
    {
        string query = @"SELECT TOP 1 * FROM Salas ORDER BY Orden";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.QueryFirstOrDefault<Sala>(query);
        }
    }

    // OBTENER LA SALA SIGUIENTE A UN ORDEN DADO
    public Sala? ObtenerSalaSiguiente(int ordenActual)
    {
        string query = @"SELECT TOP 1 * FROM Salas
                         WHERE Orden > @OrdenActual ORDER BY Orden";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.QueryFirstOrDefault<Sala>(query, new { OrdenActual = ordenActual });
        }
    }

    // =========================================================
    //  PROGRESO POR SALA
    // =========================================================

    // CREAR EL PROGRESO DE UNA SALA (sirve tanto para la sala inicial como para las siguientes)
    public void CrearProgreso(int partidaId, int salaId)
    {
        string query = @"INSERT INTO ProgresoPorSala
                        (PartidaId, SalaId, Estado, IntentosUsados, PistasSolicitadas, FechaUltimaAccion)
                        VALUES
                        (@PartidaId, @SalaId, 'Desbloqueada', 0, 0, @FechaUltimaAccion)";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new { PartidaId = partidaId, SalaId = salaId, FechaUltimaAccion = DateTime.Now });
        }
    }

    // OBTENER EL PROGRESO DE UNA SALA PUNTUAL
    public ProgresoPorSala? ObtenerProgreso(int partidaId, int salaId)
    {
        string query = @"SELECT * FROM ProgresoPorSala
                         WHERE PartidaId = @PartidaId AND SalaId = @SalaId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.QueryFirstOrDefault<ProgresoPorSala>(query, new { PartidaId = partidaId, SalaId = salaId });
        }
    }

    // OBTENER LA SALA "ACTUAL" DE LA PARTIDA (la que está Desbloqueada y no completada)
    public int ObtenerSalaActual(int partidaId)
    {
        string query = @"SELECT TOP 1 SalaId FROM ProgresoPorSala
                         WHERE PartidaId = @PartidaId AND Estado = 'Desbloqueada'
                         ORDER BY SalaId DESC";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.QueryFirstOrDefault<int>(query, new { PartidaId = partidaId });
        }
    }

    // VALIDAR SI EL JUGADOR PUEDE ENTRAR A UNA SALA (existe progreso creado para ella)
    public bool PuedeAccederASala(int partidaId, int salaId)
    {
        string query = @"SELECT COUNT(1) FROM ProgresoPorSala
                         WHERE PartidaId = @PartidaId AND SalaId = @SalaId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            int count = connection.QueryFirstOrDefault<int>(query, new { PartidaId = partidaId, SalaId = salaId });
            return count > 0;
        }
    }

    // SUMAR UN INTENTO
    public void SumarIntento(int partidaId, int salaId)
    {
        string query = @"UPDATE ProgresoPorSala
                         SET IntentosUsados = IntentosUsados + 1, FechaUltimaAccion = @FechaUltimaAccion
                         WHERE PartidaId = @PartidaId AND SalaId = @SalaId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new { PartidaId = partidaId, SalaId = salaId, FechaUltimaAccion = DateTime.Now });
        }
    }

    // SUMAR UNA PISTA
    public void SumarPista(int partidaId, int salaId)
    {
        string query = @"UPDATE ProgresoPorSala
                         SET PistasSolicitadas = PistasSolicitadas + 1, FechaUltimaAccion = @FechaUltimaAccion
                         WHERE PartidaId = @PartidaId AND SalaId = @SalaId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new { PartidaId = partidaId, SalaId = salaId, FechaUltimaAccion = DateTime.Now });
        }
    }

    // MARCAR UNA SALA COMO COMPLETADA
    public void CompletarSala(int partidaId, int salaId)
    {
        string query = @"UPDATE ProgresoPorSala
                         SET Estado = 'Completada', FechaUltimaAccion = @FechaUltimaAccion
                         WHERE PartidaId = @PartidaId AND SalaId = @SalaId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new { PartidaId = partidaId, SalaId = salaId, FechaUltimaAccion = DateTime.Now });
        }
    }
}
using Dapper;
using Microsoft.Data.SqlClient;

namespace Tp06.Models;

public class BD
{
    private string _connectionString = @"Server=localhost;DataBase=TP06Prog;Integrated Security=True;TrustServerCertificate=True;";


    // CREAR UNA PARTIDA
    public void CrearPartida(string nombreParticipante)
    {
        string query = @"INSERT INTO Partidas
                        (NombreParticipante, FechaInicio, FechaFin, Estado, TiempoConsumido)
                        VALUES
                        (@NombreParticipante, @FechaInicio, NULL, @Estado, NULL)";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new
            {
                NombreParticipante = nombreParticipante,
                FechaInicio = DateTime.Now,
                Estado = "Activa"
            });
        }
    }


    // OBTENER EL ID DE LA ULTIMA PARTIDA CREADA POR ESE JUGADOR
    public int ObtenerUltimaPartida(string nombreParticipante)
    {
        string query = @"SELECT TOP 1 PartidaId
                         FROM Partidas
                         WHERE NombreParticipante = @NombreParticipante
                         ORDER BY PartidaId DESC";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.QueryFirst<int>(query, new
            {
                NombreParticipante = nombreParticipante
            });
        }
    }


    // OBTENER UNA PARTIDA POR SU ID
    public Partida? ObtenerPartida(int partidaId)
    {
        string query = @"SELECT *
                         FROM Partidas
                         WHERE PartidaId = @PartidaId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.QueryFirstOrDefault<Partida>(query, new
            {
                PartidaId = partidaId
            });
        }
    }


    // OBTENER LAS PARTIDAS DE UN JUGADOR
    public List<Partida> ObtenerPartidas(string nombreParticipante)
    {
        string query = @"SELECT *
                         FROM Partidas
                         WHERE NombreParticipante = @NombreParticipante
                         ORDER BY PartidaId DESC";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.Query<Partida>(query, new
            {
                NombreParticipante = nombreParticipante
            }).ToList();
        }
    }


    // CREAR EL PROGRESO DE LA PRIMERA SALA
    public void CrearProgresoInicial(int partidaId)
    {
        string query = @"INSERT INTO ProgresoPorSala
                        (PartidaId, SalaId, Estado, IntentosUsados, PistasSolicitadas, FechaUltimaAccion)
                        VALUES
                        (@PartidaId, 1, @Estado, 0, 0, @FechaUltimaAccion)";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new
            {
                PartidaId = partidaId,
                Estado = "Desbloqueada",
                FechaUltimaAccion = DateTime.Now
            });
        }
    }


    // OBTENER UNA SALA
    public Sala? ObtenerSala(int salaId)
    {
        string query = @"SELECT *
                         FROM Salas
                         WHERE SalaId = @SalaId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.QueryFirstOrDefault<Sala>(query, new
            {
                SalaId = salaId
            });
        }
    }


    // OBTENER EL PROGRESO DE UNA SALA
    public ProgresoPorSala? ObtenerProgreso(int partidaId, int salaId)
    {
        string query = @"SELECT *
                         FROM ProgresoPorSala
                         WHERE PartidaId = @PartidaId
                         AND SalaId = @SalaId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.QueryFirstOrDefault<ProgresoPorSala>(query, new
            {
                PartidaId = partidaId,
                SalaId = salaId
            });
        }
    }


    // SUMAR UN INTENTO
    public void SumarIntento(int partidaId, int salaId)
    {
        string query = @"UPDATE ProgresoPorSala
                         SET IntentosUsados = IntentosUsados + 1,
                             FechaUltimaAccion = @FechaUltimaAccion
                         WHERE PartidaId = @PartidaId
                         AND SalaId = @SalaId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new
            {
                PartidaId = partidaId,
                SalaId = salaId,
                FechaUltimaAccion = DateTime.Now
            });
        }
    }


    // SUMAR UNA PISTA
    public void SumarPista(int partidaId, int salaId)
    {
        string query = @"UPDATE ProgresoPorSala
                         SET PistasSolicitadas = PistasSolicitadas + 1,
                             FechaUltimaAccion = @FechaUltimaAccion
                         WHERE PartidaId = @PartidaId
                         AND SalaId = @SalaId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new
            {
                PartidaId = partidaId,
                SalaId = salaId,
                FechaUltimaAccion = DateTime.Now
            });
        }
    }


    // MARCAR UNA SALA COMO COMPLETADA
    public void CompletarSala(int partidaId, int salaId)
    {
        string query = @"UPDATE ProgresoPorSala
                         SET Estado = @Estado,
                             FechaUltimaAccion = @FechaUltimaAccion
                         WHERE PartidaId = @PartidaId
                         AND SalaId = @SalaId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new
            {
                Estado = "Completada",
                FechaUltimaAccion = DateTime.Now,
                PartidaId = partidaId,
                SalaId = salaId
            });
        }
    }


    // CREAR EL PROGRESO DE LA SIGUIENTE SALA
    public void CrearProgresoSala(int partidaId, int salaId)
    {
        string query = @"INSERT INTO ProgresoPorSala
                        (PartidaId, SalaId, Estado, IntentosUsados, PistasSolicitadas, FechaUltimaAccion)
                        VALUES
                        (@PartidaId, @SalaId, @Estado, 0, 0, @FechaUltimaAccion)";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new
            {
                PartidaId = partidaId,
                SalaId = salaId,
                Estado = "Desbloqueada",
                FechaUltimaAccion = DateTime.Now
            });
        }
    }


    // BUSCAR LA SALA SIGUIENTE
    public Sala? ObtenerSalaSiguiente(int ordenActual)
    {
        string query = @"SELECT TOP 1 *
                         FROM Salas
                         WHERE Orden > @OrdenActual
                         ORDER BY Orden";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.QueryFirstOrDefault<Sala>(query, new
            {
                OrdenActual = ordenActual
            });
        }
    }


    // OBTENER LA SALA ACTUAL DE UNA PARTIDA
    public int ObtenerSalaActual(int partidaId)
    {
        string query = @"SELECT TOP 1 SalaId
                         FROM ProgresoPorSala
                         WHERE PartidaId = @PartidaId
                         AND Estado = @Estado
                         ORDER BY SalaId DESC";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.QueryFirstOrDefault<int>(query, new
            {
                PartidaId = partidaId,
                Estado = "Desbloqueada"
            });
        }
    }


    // GUARDAR EL TIEMPO CONSUMIDO
    public void ActualizarTiempo(int partidaId, int tiempoConsumido)
    {
        string query = @"UPDATE Partidas
                         SET TiempoConsumido = @TiempoConsumido
                         WHERE PartidaId = @PartidaId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new
            {
                TiempoConsumido = tiempoConsumido,
                PartidaId = partidaId
            });
        }
    }


    // FINALIZAR UNA PARTIDA GANADA
    public void FinalizarPartida(int partidaId, int tiempoConsumido)
    {
        string query = @"UPDATE Partidas
                         SET Estado = @Estado,
                             FechaFin = @FechaFin,
                             TiempoConsumido = @TiempoConsumido
                         WHERE PartidaId = @PartidaId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new
            {
                Estado = "Completada",
                FechaFin = DateTime.Now,
                TiempoConsumido = tiempoConsumido,
                PartidaId = partidaId
            });
        }
    }


    // MARCAR UNA PARTIDA COMO VENCIDA
    public void VencerPartida(int partidaId, int tiempoConsumido)
    {
        string query = @"UPDATE Partidas
                         SET Estado = @Estado,
                             FechaFin = @FechaFin,
                             TiempoConsumido = @TiempoConsumido
                         WHERE PartidaId = @PartidaId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new
            {
                Estado = "Vencida",
                FechaFin = DateTime.Now,
                TiempoConsumido = tiempoConsumido,
                PartidaId = partidaId
            });
        }
    }
}
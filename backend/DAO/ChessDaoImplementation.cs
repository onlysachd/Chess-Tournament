using WebApplication1.Models;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Diagnostics.Metrics;

using static Npgsql.Replication.PgOutput.Messages.RelationMessage;

namespace WebApplication1.DAO
{
    public class ChessDAOImpl : IChessDAO
    {
        NpgsqlConnection _connection;
        public ChessDAOImpl(NpgsqlConnection connection)
        {
            _connection = connection;
        }
        public async Task<List<Player>> GetPlayerByCountry(string country, string column)
        {

            string query = $"select * from Players where country = @country order by {column}";
            List<Player> playerList = new List<Player>();
            Player? p = null;

            NpgsqlParameter parameter1 = new()
            {
                ParameterName = "@column",
                NpgsqlDbType = NpgsqlDbType.Text,
                Direction = ParameterDirection.Input,
                Value = column
            };



            try
            {
                using (_connection)
                {
                    await _connection.OpenAsync();
                    NpgsqlCommand command = new NpgsqlCommand(query, _connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@country", country);
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            p = new Player();
                            p.PlayerID = reader.GetInt32(0);
                            p.FirstName = reader.GetString(1);
                            p.LastName = reader.GetString(2);
                            p.Country = reader.GetString(3);
                            p.CurrentWorldRanking = reader.GetInt32(4);
                            p.TotalMatchesPlayed = reader.GetInt32(5);
                            playerList.Add(p);

                        }
                    }

                    reader?.Close();
                }
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine(e.Message);
            }

            return playerList;
        }


        public async Task<List<PlayerWinPercentage>> GetPlayerWinPercentageByAverageOfWins()
        {
            string query = $"WITH PlayerWins AS (SELECT winner_id AS player_id,COUNT(*) AS wins FROM Matches WHERE winner_id IS NOT NULL GROUP BY winner_id),AverageWins AS (SELECT AVG(wins) AS avg_wins FROM PlayerWins),PlayerStats AS (SELECT p.player_id,p.first_name || ' ' || p.last_name AS full_name,COALESCE(pw.wins, 0) AS wins,(COALESCE(pw.wins, 0) * 100.0 / p.total_matches_played) AS win_percentage FROM Players p LEFT JOIN PlayerWins pw ON p.player_id = pw.player_id)SELECT ps.full_name as full_name,ps.wins as total_wins,ps.win_percentage as win_percentage FROM PlayerStats ps, AverageWins aw WHERE ps.wins > aw.avg_wins;";
            List<PlayerWinPercentage> playerList = new List<PlayerWinPercentage>();
            PlayerWinPercentage? p = null;

            try
            {
                using (_connection)
                {
                    await _connection.OpenAsync();
                    NpgsqlCommand command = new NpgsqlCommand(query, _connection);
                    command.CommandType = CommandType.Text;
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            p = new PlayerWinPercentage();
                            p.FullName = reader.GetString(0);
                            p.TotalMatchesWon = reader.GetInt32(1);
                            p.WinPercentage = reader.GetDecimal(2);

                            playerList.Add(p);
                        }
                    }

                    reader?.Close();
                }
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine(e.Message);
            }

            return playerList;
        }




        public async Task<int> AddMatch(Match m)
        {
            int rowsInserted = 0;
            string message;

            string insertQuery = @$"INSERT INTO Matches(player1_id, player2_id, match_date, match_level, winner_id) VALUES  ('{m.Player1Id}', '{m.Player2Id}','{m.MatchDate}','{m.MatchLevel}', '{m.WinnerId}');";
            Console.WriteLine("Query: " + insertQuery);
            try
            {
                using (_connection)
                {
                    await _connection.OpenAsync();
                    NpgsqlCommand insertCommand = new NpgsqlCommand(insertQuery, _connection);
                    insertCommand.CommandType = CommandType.Text;
                    rowsInserted = await insertCommand.ExecuteNonQueryAsync();
                }
            }
            catch (NpgsqlException e)
            {
                message = e.Message;
                Console.WriteLine("------Exception-----:" + message);
            }
            return rowsInserted;
        }
        public async Task<List<PlayerWinPercentage>> GetPlayerWinPercentage()
        {
            string query = $"SELECT p.first_name || ' ' || p.last_name AS full_name,p.total_matches_played as total_matches_won,ROUND((COUNT(m.winner_id) * 100.0) / NULLIF(p.total_matches_played, 0),4) AS win_percentage FROM Players p LEFT JOIN Matches m ON p.player_id = m.winner_id GROUP BY p.first_name, p.last_name, p.total_matches_played;";
            List<PlayerWinPercentage> playerList = new List<PlayerWinPercentage>();
            PlayerWinPercentage? p = null;

            try
            {
                using (_connection)
                {
                    await _connection.OpenAsync();
                    NpgsqlCommand command = new NpgsqlCommand(query, _connection);
                    command.CommandType = CommandType.Text;
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            p = new PlayerWinPercentage();
                            p.FullName = reader.GetString(0);
                            p.TotalMatchesWon = reader.GetInt32(1);
                            p.WinPercentage = reader.GetDecimal(2);
                            playerList.Add(p);
                        }
                    }

                    reader?.Close();
                }
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine(e.Message);
            }

            return playerList;
        }

    }
}

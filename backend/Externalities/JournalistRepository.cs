using Dapper;
using Externalities.ParameterModels;
using Externalities.QueryModels;
using MySql.Data.MySqlClient;

namespace Externalities;

public class JournalistRepository(string connectionString)
{
    private MySqlConnection GetOpenConnection()
    {
        var connection = new MySqlConnection(connectionString);
        connection.Open();
        return connection;
    }
    
    public Journalist AddJournalist(AddJournalistParams j)
    {
        using var connection = GetOpenConnection();
        connection.Execute($"INSERT INTO db.journalist (username) VALUES (@{nameof(j.username)});", j);
        return connection.QueryFirstOrDefault<Journalist>($"SELECT username, LAST_INSERT_ID() as {nameof(Journalist.journalistId)} from journalist where username = @username", new {username = j.username}) ??
               throw new InvalidOperationException("Failure to create new Journalist.");
    }
}
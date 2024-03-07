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
    
    // add the user to the journalist table
    public Journalist AddJournalist(AddJournalistParams j)
    {
        using var connection = GetOpenConnection();
        return connection.QueryFirstOrDefault<Journalist>($@"
INSERT INTO db.journalist (username) VALUES (@{nameof(j.username)})
returning username as {nameof(Journalist.username)}, id as {nameof(Journalist.journalistId)};", j) ??
               throw new InvalidOperationException("Failure to create new Journalist.");
    }
}
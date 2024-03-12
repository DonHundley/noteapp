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
    
    public bool DoesUsernameExist(FindByUserParams userParams)
    {
        using var connection = GetOpenConnection();
    
        var result = connection.QueryFirstOrDefault<string>(
            $"SELECT username FROM db.journalist WHERE username = @{nameof(userParams.username)}", 
            userParams);

        return result != null;
    }
    
    public Journalist AddJournalist(AddJournalistParams j)
    {
        using var connection = GetOpenConnection();
        connection.Execute($"INSERT INTO db.journalist (username, hash, salt) VALUES (@{nameof(j.username)}, @{nameof(j.hash)}, @{nameof(j.salt)});", j);
        return connection.QueryFirstOrDefault<Journalist>(
            $"SELECT username, hash, salt, LAST_INSERT_ID() as {nameof(Journalist.journalistId)} from journalist where username = @username",
            new { username = j.username });
    }
    
    public Journalist GetJournalist(FindByUserParams userParams)
    {
        using var connection = GetOpenConnection();

        return connection.QueryFirstOrDefault<Journalist>(
            $"SELECT id AS journalistId, username, hash, salt FROM db.journalist WHERE username = @{nameof(userParams.username)}",
            userParams);
    }
}
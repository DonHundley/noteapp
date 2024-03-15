using Dapper;
using Externalities.ParameterModels;
using Externalities.QueryModels;
using MySql.Data.MySqlClient;
using Serilog;

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

        try
        {
           var result = connection.QueryFirstOrDefault<string>(
                       $"SELECT username FROM db.journalist WHERE username = @{nameof(userParams.username)}", 
                       userParams);
           
                   return result != null; 
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while checking if a username exists in JournalistRepository");
            throw new Exception();
        }
        
    }
    
    public Journalist AddJournalist(AddJournalistParams j)
    {
        try
        {
            using var connection = GetOpenConnection();
            connection.Execute(
                $"INSERT INTO db.journalist (username, hash, salt) VALUES (@{nameof(j.username)}, @{nameof(j.hash)}, @{nameof(j.salt)});",
                j);
            return connection.QueryFirstOrDefault<Journalist>(
                $"SELECT username, hash, salt, LAST_INSERT_ID() as {nameof(Journalist.journalistId)} from journalist where username = @username",
                new { username = j.username });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while adding a journalist in JournalistRepository");
            throw new Exception();
        }
    }
    
    public Journalist GetJournalist(FindByUserParams userParams)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.QueryFirstOrDefault<Journalist>(
                $"SELECT id AS journalistId, username, hash, salt FROM db.journalist WHERE username = @{nameof(userParams.username)}",
                userParams);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while getting a journalist in JournalistRepository");
            throw new Exception();
        }
    }
}
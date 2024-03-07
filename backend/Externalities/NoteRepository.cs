using Dapper;
using Externalities.QueryModels;
using MySql.Data.MySqlClient; 



public class NoteRepository
{
    private readonly string _connectionString;

    public NoteRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private MySqlConnection GetOpenConnection()
    {
        var connection = new MySqlConnection(_connectionString);
        connection.Open();
        return connection;
    }
    
    public async Task Add(Note note)
    {
        const string sql = "INSERT INTO Notes(Content, Topic) VALUES(@Content, @Topic);";

        using var connection = GetOpenConnection();
        
        
        await connection.ExecuteAsync(sql, note);
    }

    public async Task<Note> Get(int id)
    {
        const string sql = "SELECT * FROM Notes WHERE Id = @Id;";

        using var connection = GetOpenConnection();

        return await connection.QuerySingleOrDefaultAsync<Note>(sql, new {Id = id});
    }

    public async Task Update(Note note)
    {
        const string sql = "UPDATE Notes SET Content = @Content, Topic = @Topic WHERE Id = @Id;";

        using var connection = GetOpenConnection();

        await connection.ExecuteAsync(sql, note);
    }

    public async Task Delete(int id)
    {
        const string sql = "DELETE FROM Notes WHERE Id = @Id;";

        using var connection = GetOpenConnection();

        await connection.ExecuteAsync(sql, new {Id = id});
    }
}
using Dapper;
using Externalities.ParameterModels;
using Externalities.QueryModels;
using MySql.Data.MySqlClient;

namespace Externalities;

public class NoteRepository(string connectionString)
{
    private MySqlConnection GetOpenConnection()
    {
        var connection = new MySqlConnection(connectionString);
        connection.Open();
        return connection;
    }


    public IEnumerable<Note> GetNotesBySubject(GetNotesParams getNotesParams)
    {
        using var connection = GetOpenConnection();
        return connection.Query<Note>($@"
SELECT 
    n.noteContent as {nameof(Note.noteContent)},
    n.sender as {nameof(Note.sender)},
    n.id as {nameof(Note.id)}, 
    n.timestamp as {nameof(Note.timestamp)}, 
    n.subjectId as {nameof(Note.subjectId)}
FROM notes n
JOIN journalist j ON n.sender = j.id 
WHERE n.id < @{nameof(GetNotesParams.lastNoteId)} AND 
      n.subjectId = @{nameof(GetNotesParams.subjectId)}
ORDER BY n.timestamp DESC;", getNotesParams);
    }

    public async Task Add(Note note)
    {
        const string sql = "INSERT INTO Notes(Content, Subject) VALUES(@Content, @Topic);";
        using var connection = GetOpenConnection();
        await connection.ExecuteAsync(sql, note);
    }

    public async Task<Note> Get(int id)
    {
        const string sql = "SELECT * FROM Notes WHERE Id = @Id;";
        using var connection = GetOpenConnection();
        return await connection.QuerySingleOrDefaultAsync<Note>(sql, new { Id = id });
    }

    public async Task Update(Note note)
    {
        const string sql = "UPDATE Notes SET Content = @Content, Subject = @Subject WHERE Id = @Id;";
        using var connection = GetOpenConnection();
        await connection.ExecuteAsync(sql, note);
    }

    public async Task Delete(int id)
    {
        const string sql = "DELETE FROM Notes WHERE Id = @Id;";
        using var connection = GetOpenConnection();
        await connection.ExecuteAsync(sql, new { Id = id });
    }
}
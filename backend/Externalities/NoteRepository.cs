using Dapper;
using Externalities.ParameterModels;
using Externalities.QueryModels;
using MySql.Data.MySqlClient;
using Serilog;

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
        try
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
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while getting notes by subject in NoteRepository");
            throw new Exception();
        }
    }



    public async Task<Note> Add(CreateNoteParams createParams)
    {
        const string sql = @"
INSERT INTO db.notes(noteContent, timestamp, subjectId, sender) 
VALUES(@noteContent, @timestamp, @subjectId, @sender);
SELECT LAST_INSERT_ID();";

        try
        {
            using var connection = GetOpenConnection();
            int id = await connection.ExecuteAsync(sql, createParams);
    
            var newNote = new Note
            {
                id = id,
                noteContent = createParams.noteContent,
                timestamp = createParams.timestamp,
                subjectId = createParams.subjectId,
                sender = createParams.sender
            };

            return newNote;
        }
        catch(Exception ex)
        {
            Log.Error(ex, "An error occurred while adding a note in NoteRepository");
            throw new Exception();
        }
    }
    

    public async Task<Note> Update(EditNoteParams editParams)
    {
        const string sql = @"
UPDATE db.notes
SET noteContent = @noteContent, 
    timestamp = @timestamp, 
    subjectId = @subjectId, 
    sender = @sender
WHERE id = @id;
SELECT * FROM db.notes WHERE id = @id;";

        try
        {
            using var connection = GetOpenConnection();
            var updatedNote = await connection.QueryFirstOrDefaultAsync<Note>(sql, editParams);

            if(updatedNote == null)
            {
                throw new Exception("No note found with the specified ID");
            }

            return updatedNote;
        }
        catch(Exception ex)
        {
            Log.Error(ex, "An error occurred while updating a note in NoteRepository");
            throw new Exception();
        }
    }

    public async Task<bool> Delete(DeleteNotesParams deleteParams)
    {
        const string sql = "DELETE FROM db.notes WHERE id = @id;";

        try
        {
            using var connection = GetOpenConnection();
            var result = await connection.ExecuteAsync(sql, deleteParams);

            // If no rows were affected, return false. Otherwise, return true.
            return result > 0;
        }
        catch(Exception ex)
        {
            Log.Error(ex, "An error occurred while deleting a note in NoteRepository");
            throw new Exception();
        }
    }
}
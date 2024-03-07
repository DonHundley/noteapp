using api.Models.Enums;
using Externalities.QueryModels;
using Fleck;

namespace api.State;

public class WsWithMetadata(IWebSocketConnection connection)
{
    public IWebSocketConnection Connection { get; set; } = connection;
}

public class WebSocketStateService
{
    private static readonly Dictionary<Guid, WsWithMetadata> _clients = new();
    // Dictionary of subjects where notes will be posted
    private static readonly Dictionary<SubjectEnums, List<Note>> _subjects = new();
    
    // Add a client to the dictionary
    public static void AddClient(Guid clientId, IWebSocketConnection connection)
    {
        _clients.TryAdd(clientId, new WsWithMetadata(connection));
    }
    // Remove a client from the dictionary
    public static void RemoveClient(Guid clientId)
    {
        _clients.Remove(clientId);
    }
    // Get all clients from the dictionary
    public static IEnumerable<WsWithMetadata> GetAllClients()
    {
        return _clients.Values;
    }
    // Get a client by ID from the dictionary
    public static WsWithMetadata GetClient(Guid clientId)
    {
        _clients.TryGetValue(clientId, out var client);
        return client;
    }
    // Add a note to a SubjectEnums in _subjects
    public static void AddNoteToSubject(SubjectEnums subject, Note note)
    {
        if (!_subjects.ContainsKey(subject))
        {
            _subjects[subject] = new List<Note>();
        }
        _subjects[subject].Add(note);
    }
    
}
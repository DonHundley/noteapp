using api.Exceptions;
using api.Helpers;
using api.Models.Enums;
using api.ServerEvents;
using Externalities.QueryModels;
using Fleck;
using lib;

namespace api.State;

public class WsWithMetadata(IWebSocketConnection connection)
{
    public IWebSocketConnection Connection { get; set; } = connection;
}

public class WebSocketStateService
{
    // Dictionary that maintains the map of a Guid to a specific websocket
    private static readonly Dictionary<Guid, WsWithMetadata> _clients = new();
    // Dictionary that maintains a map between each client and each subject they are subscribed to
    private static readonly Dictionary<Guid, HashSet<SubjectEnums>> _clientSubjects = new();
    // Dictionary that is used to track which clients are subscribed to which subject
    private static readonly Dictionary<SubjectEnums, HashSet<Guid>> _subjectClients = new();
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
    
    // get clients subscribed to a subject
    public static List<IWebSocketConnection> GetClientsSubscribedToSubject(int subjectId)
    {
        if (Enum.IsDefined(typeof(SubjectEnums), subjectId))
        {
            SubjectEnums subject = (SubjectEnums)subjectId;
            return _subjectClients.TryGetValue(subject, out var clients)
                ? clients.Select(clientId => _clients[clientId].Connection).ToList()
                : new List<IWebSocketConnection>();
        }
        else
        {
            throw new InvalidEnumValueException("Invalid Subject Id");
        }
        
    }

    /// <summary>
    /// Subscribes a client to a subject.
    /// </summary>
    /// <param name="clientId">The ID of the client.</param>
    /// <param name="subjectId">The ID of the subject. Must be a valid value from the SubjectEnums enumeration.</param>
    public static void SubscribeToSubject(Guid clientId, int subjectId)
    {
        if(Enum.IsDefined(typeof(SubjectEnums), subjectId))
        {
            SubjectEnums subject = (SubjectEnums)subjectId;
            
            if (!_subjectClients.ContainsKey(subject)) _subjectClients[subject] = new HashSet<Guid>();

            _subjectClients[subject].Add(clientId);
        
            if (!_clientSubjects.ContainsKey(clientId)) _clientSubjects[clientId] = new HashSet<SubjectEnums>();
            _clientSubjects[clientId].Add(subject);
        }
        else
        {
            throw new InvalidEnumValueException("Invalid Subject Id");
        }
    }
    
    public static void AddNoteToSubject<T>(int subjectId, T dto ) where T : BaseDto
    {
        if (Enum.IsDefined(typeof(SubjectEnums), subjectId))
        {
            SubjectEnums subject = (SubjectEnums)subjectId;
            
            if(_subjectClients.TryGetValue(subject, out var clients))
                foreach (var clientId in clients)
                    if(_clients.TryGetValue(clientId, out var connection))
                        connection.Connection.SendDto(dto);
        }
        else
        {
            throw new InvalidEnumValueException("Invalid Subject Id");
        }
        
    }
    
}
using api.Exceptions;
using api.Helpers;
using api.Models.Enums;
using api.ServerEvents;
using Externalities.QueryModels;
using Fleck;
using lib;

namespace api.State;

/// <summary>
/// Represents a WebSocket connection with metadata.
/// </summary>
public class WsWithMetadata(IWebSocketConnection connection)
{
    /// <summary>
    /// Represents a WebSocket connection with additional metadata.
    /// </summary>
    public IWebSocketConnection Connection { get; set; } = connection;

    /// <summary>
    /// Represents a journalist.
    /// </summary>
    public Journalist? Journalist { get; set; }

    public bool Authorized { get; set; }
}

/// <summary>
/// This class is responsible for managing the state of web socket connections and clients.
/// </summary>
public class WebSocketStateService
{
    // Dictionary that maintains the map of a Guid to a specific websocket
    /// <summary>
    /// Represents a WebSocket connection with additional metadata.
    /// </summary>
    private static readonly Dictionary<Guid, WsWithMetadata> _clients = new();
    // Dictionary that maintains a map between each client and each subject they are subscribed to
    /// <summary>
    /// Dictionary that maintains a map between each client and each subject they are subscribed to.
    /// </summary>
    private static readonly Dictionary<Guid, HashSet<SubjectEnums>> _clientSubjects = new();
    // Dictionary that is used to track which clients are subscribed to which subject
    /// ` class.
    private static readonly Dictionary<SubjectEnums, HashSet<Guid>> _subjectClients = new();
    
    
    // Add a client to the dictionary
    /// <summary>
    /// Add a client to the dictionary of WebSocket connections.
    /// </summary>
    /// <param name="clientId">The unique ID of the client.</param>
    /// <param name="connection">The WebSocket connection.</param>
    public static void AddClient(Guid clientId, IWebSocketConnection connection)
    {
        _clients.TryAdd(clientId, new WsWithMetadata(connection));
    }
    // Remove a client from the dictionary
    /// <summary>
    /// Removes a client from the dictionary of WebSocket clients.
    /// </summary>
    /// <param name="clientId">The ID of the client to be removed.</param>
    public static void RemoveClient(Guid clientId)
    {
        _clients.Remove(clientId);
    }
    // Get all clients from the dictionary
    /// <summary>
    /// Gets all the clients from the dictionary.
    /// </summary>
    /// <returns>An IEnumerable of WsWithMetadata containing the clients.</returns>
    public static IEnumerable<WsWithMetadata> GetAllClients()
    {
        return _clients.Values;
    }
    // Get a client by ID from the dictionary
    /// <summary>
    /// Gets a client from the dictionary by ID.
    /// </summary>
    /// <param name="clientId">The ID of the client.</param>
    /// <returns>The WebSocketStateService.WsWithMetadata object associated with the client ID.</returns>
    public static WsWithMetadata GetClient(Guid clientId)
    {
        return _clients[clientId];
    }
    
    // get clients subscribed to a subject
    /// <summary>
    /// Get clients subscribed to a subject.
    /// </summary>
    /// <param name="subjectId">The ID of the subject.</param>
    /// <returns>A list of WebSocket connections of clients subscribed to the subject. Returns an empty list if no clients are subscribed.</returns>
    public static List<IWebSocketConnection> GetClientsSubscribedToSubject(int subjectId)
    {
        if (Enum.IsDefined(typeof(SubjectEnums), subjectId))
        {
            SubjectEnums subject = (SubjectEnums)subjectId;
            return _subjectClients.TryGetValue(subject, out var clients)
                ? clients.Where(clientId => _clients.ContainsKey(clientId))
                         .Select(clientId => _clients[clientId].Connection)
                         .ToList()
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
        if (Enum.IsDefined(typeof(SubjectEnums), subjectId))
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

    /// <summary>
    /// Adds a note to the specified subject.
    /// </summary>
    /// <typeparam name="T">The type of the note DTO.</typeparam>
    /// <param name="subjectId">The ID of the subject.</param>
    /// <param name="dto">The note DTO.</param>
    /// <exception cref="InvalidEnumValueException">Thrown when the subject ID is invalid.</exception>
    public static void AddNoteToSubject<T>(int subjectId, T dto ) where T : BaseDto
    {
        SubjectEnums subject = (SubjectEnums)subjectId;
        if(_subjectClients.TryGetValue(subject, out var clients)) 
            foreach (var clientId in clients) 
                if(_clients.TryGetValue(clientId, out var connection)) 
                    connection.Connection.SendDto(dto);
        
    }
    
    // a boolean that will return false if the journalist is not subscribed to the subject
    public static bool IsJournalistSubscribed(Guid clientId, int subjectId)
    {
        SubjectEnums subject = (SubjectEnums)subjectId;
        if (_clientSubjects.TryGetValue(clientId, out var subjects))
        { 
            return subjects.Contains(subject);
        }
        return false;
        
    }
    
    // a boolean that will handle checking our enum
    // utilizing this instead of a range limit on our dto so that ONLY the enum will need to be altered to add another subject
    public static bool IsValidSubject(int subjectId)
    {
        return Enum.IsDefined(typeof(SubjectEnums), subjectId);
    }
    
    
}
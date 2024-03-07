using api.Helpers;
using api.ServerEvents;
using api.State;
using Externalities;
using Externalities.ParameterModels;
using Fleck;
using lib;

namespace api.ClientEventHandlers;

public class ClientWantsToJournalDto : BaseDto
{
    public string username { get; set; }
    
}

public class ClientWantsToJournal(JournalistRepository journalistRepository) : BaseEventHandler<ClientWantsToJournalDto>
{
    public override Task Handle(ClientWantsToJournalDto dto, IWebSocketConnection socket)
    {
        var journalist = journalistRepository.AddJournalist(new AddJournalistParams(dto.username));
        WebSocketStateService.GetClient(socket.ConnectionInfo.Id).Journalist = journalist;
        socket.SendDto(new ServerAddedJournalist
        {
            message = $"Journalist {journalist.username} added successfully."
        });
        return Task.CompletedTask;
    }
}
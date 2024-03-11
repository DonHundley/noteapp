using System.ComponentModel.DataAnnotations;
using api.EventFilters;
using api.Helpers;
using api.Security;
using api.ServerEvents;
using api.State;
using Externalities;
using Externalities.ParameterModels;
using Fleck;
using lib;

namespace api.ClientEventHandlers;

public class ClientWantsToJournalDto : BaseDto
{
    [MinLength(6)] string? username { get; set; }
    [MinLength(8)] public string password { get; set; }
    
}
[AuthValidation]
public class ClientWantsToJournal(
    JournalistRepository journalistRepository,
    CredentialService credentialService,
    TokenService tokenService) : BaseEventHandler<ClientWantsToJournalDto>
{
    public override Task Handle(ClientWantsToJournalDto dto, IWebSocketConnection socket)
    {
        var journalist = journalistRepository.AddJournalist(new AddJournalistParams(dto.username));
        WebSocketStateService.GetClient(socket.ConnectionInfo.Id).Journalist = journalist;
        Console.WriteLine(WebSocketStateService.GetClient(socket.ConnectionInfo.Id).Journalist);
        socket.SendDto(new ServerAddedJournalist
        {
            message = "Welcome!"
        });
        return Task.CompletedTask;
    }
}
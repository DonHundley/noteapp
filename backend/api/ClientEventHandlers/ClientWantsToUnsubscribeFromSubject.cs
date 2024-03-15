using System.ComponentModel.DataAnnotations;
using api.EventFilters;
using api.Helpers;
using api.ServerEvents;
using api.State;
using Externalities.ParameterModels;
using Fleck;
using lib;

namespace api.ClientEventHandlers;

public class ClientWantsToUnsubscribeFromSubjectDto : BaseDto
{
    [Required] public int subjectId { get; set; }
}
[AuthValidation]
[DataValidation]
public class ClientWantsToUnsubscribeFromSubject : BaseEventHandler<ClientWantsToUnsubscribeFromSubjectDto>
{
    public override Task Handle(ClientWantsToUnsubscribeFromSubjectDto dto, IWebSocketConnection socket)
    {
        WebSocketStateService.UnsubscribeFromSubject(socket.ConnectionInfo.Id, dto.subjectId);
        
        socket.SendDto(new ServerUnsubscribesFromSubject
        {
            message = WebSocketStateService.GetClient(socket.ConnectionInfo.Id).Journalist.username + " has been removed from subject " + dto.subjectId + "."
        });
        
        return Task.CompletedTask;
    }
}
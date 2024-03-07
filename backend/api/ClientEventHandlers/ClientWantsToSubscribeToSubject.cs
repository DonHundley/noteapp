using System.ComponentModel.DataAnnotations;
using System.Text.Unicode;
using api.Helpers;
using api.Models.Enums;
using api.ServerEvents;
using api.State;
using Externalities;
using Externalities.ParameterModels;
using Fleck;
using lib;
using Microsoft.OpenApi.Extensions;

namespace api.ClientEventHandlers;

public class ClientWantsToSubscribeToSubjectDto : BaseDto
{
    [Required] [Range(1, int.MaxValue)] public int subjectId { get; set; }
}

public class ClientWantsToSubscribeToSubject(NoteRepository noteRepository) : BaseEventHandler<ClientWantsToSubscribeToSubjectDto>
{
    public override Task Handle(ClientWantsToSubscribeToSubjectDto dto, IWebSocketConnection socket)
    {
        WebSocketStateService.SubscribeToSubject(socket.ConnectionInfo.Id, dto.subjectId);
        socket.SendDto(new ServerSubscribesClientToSubject
        {
            notes = noteRepository.GetNotesBySubject(new GetNotesParams(dto.subjectId)),
            connections = WebSocketStateService.GetClientsSubscribedToSubject(dto.subjectId).Count,
            subjectId = dto.subjectId
        });
        return Task.CompletedTask;
    }
}
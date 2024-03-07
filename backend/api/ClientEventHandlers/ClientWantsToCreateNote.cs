using System.Reactive.Subjects;
using api.Helpers;
using api.Models.Enums;
using api.ServerEvents;
using api.State;
using Externalities.QueryModels;
using Fleck;
using lib;
namespace api.ClientEventHandlers;

public class ClientWantsToCreateNoteDto : BaseDto
{
   public string? messageContent { get; set; } 
   public int subjectId { get; set; }
}

public class ClientWantsToCreateNote() : BaseEventHandler<ClientWantsToCreateNoteDto>
{
   public override Task Handle(ClientWantsToCreateNoteDto dto, IWebSocketConnection socket)
   {
      
      var note = new Note
      {
         timestamp = DateTimeOffset.UtcNow,
         noteContent = dto.messageContent,
         subjectId = dto.subjectId
      };
      
      WebSocketStateService.AddNoteToSubject(dto.subjectId, new ServerAddsNoteToSubject
      {
         note = note,
         subjectId = dto.subjectId
      });
     
      
      return Task.CompletedTask;

   }
}



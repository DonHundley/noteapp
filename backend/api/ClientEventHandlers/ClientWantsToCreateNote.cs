using System.Reactive.Subjects;
using api.Models.Enums;
using api.State;
using Externalities.QueryModels;
using Fleck;
using lib;
namespace api.ClientEventHandlers;

public class ClientWantsToCreateNoteDto : BaseDto
{
   public string? messageContent { get; set; } 
   public SubjectEnums? subject { get; set; }
}

public class ClientWantsToCreateNote(NoteRepository noteRepository) : BaseEventHandler<ClientWantsToCreateNoteDto>
{
   public override Task Handle(ClientWantsToCreateNoteDto dto, IWebSocketConnection socket)
   {
      
      var note = new Note
      {
         timestamp = DateTimeOffset.UtcNow,
         Content = dto.messageContent,
         Subject= dto.subject.ToString()
      };
      
      WebSocketStateService.AddNoteToSubject(dto.subject.Value, note);
      
      
      return Task.CompletedTask;

   }
}

public class ServerCreatesNote : BaseDto
{
   public string message { get; set; }
}

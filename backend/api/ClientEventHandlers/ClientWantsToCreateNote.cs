using System.ComponentModel.DataAnnotations;
using System.Reactive.Subjects;
using api.EventFilters;
using api.Exceptions;
using api.Helpers;
using api.Models.Enums;
using api.ServerEvents;
using api.State;
using Externalities;
using Externalities.ParameterModels;
using Externalities.QueryModels;
using Fleck;
using lib;
using Serilog;

namespace api.ClientEventHandlers;

public class ClientWantsToCreateNoteDto : BaseDto
{
   [Required] [MinLength(1)] public string? messageContent { get; set; } 
   
   [Required] public int subjectId { get; set; }
}
[AuthValidation]
[DataValidation]
public class ClientWantsToCreateNote(NoteRepository noteRepository) : BaseEventHandler<ClientWantsToCreateNoteDto>
{
   public override async Task Handle(ClientWantsToCreateNoteDto dto, IWebSocketConnection socket)
   {
      
      
      // is the subject valid?
      // We are checking this here because if more subjects are added I want this to be flexible
      if (!WebSocketStateService.IsValidSubject(dto.subjectId))
      {
         
         throw new InvalidEnumValueException("There is not a subject for that SubjectId!");
      }
      // are they subscribed to the subject?
      if (!WebSocketStateService.IsJournalistSubscribed(socket.ConnectionInfo.Id, dto.subjectId))
      {
         throw new SubException("Journalist is not subscribed to the subject!");
      }
      
      
      try
      {
         var noteParam = new CreateNoteParams
         {
            timestamp = DateTimeOffset.UtcNow,
            noteContent = dto.messageContent,
            subjectId = dto.subjectId,
            sender = WebSocketStateService.GetClient(socket.ConnectionInfo.Id).Journalist.journalistId
         };
         
         
         var addNote = await noteRepository.Add(noteParam);
         
         var note = new Note
         {
            id = addNote.id,
            noteContent = dto.messageContent,
            timestamp = DateTimeOffset.UtcNow,
            subjectId = dto.subjectId,
            sender = WebSocketStateService.GetClient(socket.ConnectionInfo.Id).Journalist.journalistId
         };
         
         // LET THEM TAKE NOTES!
         WebSocketStateService.AddNoteToSubject(dto.subjectId, new ServerAddsNoteToSubject
         {
            note = note,
            subjectId = dto.subjectId
         });
      }
      catch (Exception e)
      {
         // we didn't let them take notes.
         Log.Error(e, "An error occured while adding note to repository in ClientWantsToCreateNote");
         throw new RepositoryGeneralException("There was a problem adding the note to the database!");
      }
   }
}



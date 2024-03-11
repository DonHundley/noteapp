using System.ComponentModel.DataAnnotations;
using System.Reactive.Subjects;
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
namespace api.ClientEventHandlers;

public class ClientWantsToCreateNoteDto : BaseDto
{
   [Required] [MinLength(1)] [MaxLength(255)] public string? messageContent { get; set; } 
   
   [Required] public int subjectId { get; set; }
}

public class ClientWantsToCreateNote(NoteRepository noteRepository) : BaseEventHandler<ClientWantsToCreateNoteDto>
{
   public override Task Handle(ClientWantsToCreateNoteDto dto, IWebSocketConnection socket)
   {
      
      
      // is the subject valid?
      // shouldn't be needed as we are moderating this in our dto as well.
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

         var addNote = noteRepository.Add(noteParam);

         var note = new Note
         {
            id = addNote.Result.id,
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
         throw new RepositoryGeneralException("There was a problem adding the note to the database!");
      }
      // Success! *dance*
      return Task.CompletedTask;

   }
}



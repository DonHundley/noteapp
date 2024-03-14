using System.ComponentModel.DataAnnotations;
using api.EventFilters;
using api.Exceptions;
using api.ServerEvents;
using api.State;
using Externalities;
using Externalities.ParameterModels;
using Externalities.QueryModels;
using Fleck;
using lib;
using Serilog;

namespace api.ClientEventHandlers;

public class ClientWantsToEditNoteDto : BaseDto
{
    [Required] public int id { get; set; }
    [Required] [MinLength(1)]  public string? messageContent { get; set; } 
    [Required] public int subjectId { get; set; }
}
[AuthValidation]
[DataValidation]
public class ClientWantsToEditNote(NoteRepository noteRepository) : BaseEventHandler<ClientWantsToEditNoteDto>
{
    public override async Task Handle(ClientWantsToEditNoteDto dto, IWebSocketConnection socket)
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
            var noteParam = new EditNoteParams
            {
                id = dto.id,
                timestamp = DateTimeOffset.UtcNow,
                noteContent = dto.messageContent,
                subjectId = dto.subjectId,
                sender = WebSocketStateService.GetClient(socket.ConnectionInfo.Id).Journalist.journalistId
            };


            var updateNote = await noteRepository.Update(noteParam);

            var note = new Note
            {
                id = updateNote.id,
                noteContent = updateNote.noteContent,
                timestamp = updateNote.timestamp,
                subjectId = updateNote.subjectId,
                sender = WebSocketStateService.GetClient(socket.ConnectionInfo.Id).Journalist.journalistId
            };

            
            WebSocketStateService.AddNoteToSubject(dto.subjectId, new ServerUpdatesNoteInSubject
            {
                note = note,
                subjectId = dto.subjectId
            });
        }
        catch (Exception e)
        {
            
            Log.Error(e, "An error occured while adding note to repository in ClientWantsToUpdateNote");
            throw new RepositoryGeneralException("There was a problem updating the note in the database!");
        }
    }
}
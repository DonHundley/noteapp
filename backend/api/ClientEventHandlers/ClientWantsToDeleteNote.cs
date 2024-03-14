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

public class ClientWantsToDeleteNoteDto : BaseDto
{
    [Required] public int id { get; set; }
    [Required] public int subjectId { get; set; }
}
[AuthValidation]
[DataValidation]
public class ClientWantsToDeleteNote(NoteRepository noteRepository) : BaseEventHandler<ClientWantsToDeleteNoteDto>
{
    public override async Task Handle(ClientWantsToDeleteNoteDto dto, IWebSocketConnection socket)
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
            var noteParam = new DeleteNotesParams
            {
                id = dto.id
            };

            
            var updateNote = await noteRepository.Delete(noteParam);
            
            if (updateNote)
            {
                WebSocketStateService.AddNoteToSubject(dto.subjectId, new ServerDeletesNoteInSubject
                {
                    id = dto.id,
                    subjectId = dto.subjectId
                });
            }
            else
            {
                throw new RepositoryGeneralException("There was a problem deleting the note from the database!");
            }
            
            
        }
        catch (Exception e)
        {
            
            Log.Error(e, "An error occured while deleting note to repository in ClientWantsToDeleteNote");
            throw new RepositoryGeneralException("There was a problem deleting the note from the database!");
        }
    }
}
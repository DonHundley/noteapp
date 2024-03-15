using Externalities.QueryModels;
using lib;

namespace api.ServerEvents;

public class ServerUpdatesNoteInSubject : BaseDto
{
    public Note? note { get; set; }
    public int subjectId { get; set; }
}
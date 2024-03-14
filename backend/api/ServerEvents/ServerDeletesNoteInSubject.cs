using lib;

namespace api.ServerEvents;

public class ServerDeletesNoteInSubject : BaseDto
{
    public int id { get; set; }
    public int subjectId { get; set; }
}
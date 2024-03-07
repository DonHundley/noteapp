using Externalities.QueryModels;
using lib;

namespace api.ServerEvents;

public class ServerAddsNoteToSubject : BaseDto
{
    public Note? note { get; set; }
}
using Externalities.QueryModels;
using lib;

namespace api.ServerEvents;

public class ServerSubscribesClientToSubject : BaseDto
{
    public int subjectId { get; set; }
    public int connections { get; set; }
    public IEnumerable<Note>? notes { get; set; }
}
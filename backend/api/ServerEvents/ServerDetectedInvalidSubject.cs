using lib;

namespace api.ServerEvents;

public class ServerDetectedInvalidSubject : BaseDto
{
    public string message { get; set; }
}
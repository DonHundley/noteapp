using lib;

namespace api.ServerEvents;

public class ServerAddedJournalist : BaseDto
{
    public string message { get; set; }
}
using lib;

namespace api.ServerEvents;

public class ServerUnsubscribesFromSubject : BaseDto
{
    public string? message { get; set; }
}
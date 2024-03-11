using lib;

namespace api.ServerEvents;

public class ServerAuthenticatesJournalist : BaseDto
{
    public string? token { get; set; }
}
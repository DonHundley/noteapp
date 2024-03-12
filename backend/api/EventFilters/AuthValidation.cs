using System.Security.Authentication;
using api.State;
using Fleck;
using lib;

namespace api.EventFilters;

public class AuthValidation : BaseEventFilter
{
    public override Task Handle<T>(IWebSocketConnection socket, T dto)
    {
        if (!WebSocketStateService.GetClient(socket.ConnectionInfo.Id).Authorized)
        {
            throw new AuthenticationException("Not Authorized");
        }
        return Task.CompletedTask;
    }
}
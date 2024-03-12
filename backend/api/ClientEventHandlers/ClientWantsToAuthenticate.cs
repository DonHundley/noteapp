using System.ComponentModel.DataAnnotations;
using api.Helpers;
using api.Security;
using api.ServerEvents;
using api.State;
using Externalities;
using Externalities.ParameterModels;
using Fleck;
using lib;

namespace api.ClientEventHandlers;

public class ClientWantsToAuthenticateDTO : BaseDto
{
    [Required] public string? token { get; set; }
}

public class ClientWantsToAuthenticate(
    JournalistRepository journalistRepository,
    TokenService tokenService
    ) : BaseEventHandler<ClientWantsToAuthenticateDTO>
{
    public override async Task Handle(ClientWantsToAuthenticateDTO dto, IWebSocketConnection socket)
    {
        var claims = tokenService.ValidateJwt(dto.token!);
        var username = claims["username"];
        var userParams = new FindByUserParams(username);
        var journalist = journalistRepository.GetJournalist(userParams);
        
        if (journalist == null)
        {
            throw new InvalidOperationException("Journalist not found");
        }

        WebSocketStateService.GetClient(socket.ConnectionInfo.Id).Journalist = journalist;
        WebSocketStateService.GetClient(socket.ConnectionInfo.Id).Authorized = true;
        socket.SendDto(new ServerAuthenticatesJournalistJwt());

    }
}
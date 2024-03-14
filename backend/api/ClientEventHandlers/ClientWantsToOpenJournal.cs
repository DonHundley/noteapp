using api.EventFilters;
using api.Exceptions;
using api.Helpers;
using api.Security;
using api.ServerEvents;
using api.State;
using Externalities;
using Externalities.ParameterModels;
using Fleck;
using lib;

namespace api.ClientEventHandlers;

public class ClientWantsToOpenJournalDto : BaseDto
{
    public string username { get; set; }
    public string password { get; set; }
}
// LOGIN
[DataValidation]
public class ClientWantsToOpenJournal(
    JournalistRepository journalistRepository,
    TokenService tokenService,
    CredentialService credentialService) : BaseEventHandler<ClientWantsToOpenJournalDto>
{
    public override Task Handle(ClientWantsToOpenJournalDto dto, IWebSocketConnection socket)
    {
        // get and check the user
        var journalist = journalistRepository.GetJournalist(new FindByUserParams(dto.username));
        if (journalist == null)
        {
            throw new InvalidLoginException("Failed to sign-in, invalid credentials.");
        }
        // get and check the hash
        var hashedPassword = credentialService.Hash(dto.password, journalist.salt);
        if (hashedPassword != journalist.hash)
        {
            throw new InvalidLoginException("Failed to sign-in, invalid credentials.");
        }
        // set our variables
        WebSocketStateService.GetClient(socket.ConnectionInfo.Id).Authorized = true;
        WebSocketStateService.GetClient(socket.ConnectionInfo.Id).Journalist = journalist;
        socket.SendDto(new ServerAuthenticatesJournalist
        {
            token = tokenService.IssueToken(journalist)
        });

        return Task.CompletedTask;
    }
}
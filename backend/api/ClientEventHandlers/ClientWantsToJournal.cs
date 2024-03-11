using System.ComponentModel.DataAnnotations;
using api.EventFilters;
using api.Helpers;
using api.Security;
using api.ServerEvents;
using api.State;
using Externalities;
using Externalities.ParameterModels;
using Fleck;
using lib;

namespace api.ClientEventHandlers;

public class ClientWantsToJournalDto : BaseDto
{
    [MinLength(6)] public string username { get; set; }
    [MinLength(8)] public string password { get; set; }
    
}

[AuthValidation]
[DataValidation]
public class ClientWantsToJournal(
    JournalistRepository journalistRepository,
    CredentialService credentialService,
    TokenService tokenService) : BaseEventHandler<ClientWantsToJournalDto>
{
    public override Task Handle(ClientWantsToJournalDto dto, IWebSocketConnection socket)
    {
        if (journalistRepository.DoesUsernameExist(new FindByUserParams(dto.username)))
        {
            throw new ValidationException("Username is taken");
        }
        
        
        var salt = credentialService.GenerateSalt();
        var hash = credentialService.Hash(dto.password, salt);
        var journalist =
            journalistRepository.AddJournalist(new AddJournalistParams(dto.username, hash,
                salt));
        var issuedToken = tokenService.IssueToken(journalist);
        WebSocketStateService.GetClient(socket.ConnectionInfo.Id).Authorized = true;
        WebSocketStateService.GetClient(socket.ConnectionInfo.Id).Journalist = journalist;
        
        socket.SendDto(new ServerAuthenticatesJournalist()
        {
            token = issuedToken
        });
        return Task.CompletedTask;
    }
}
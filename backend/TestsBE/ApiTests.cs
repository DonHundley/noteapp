using System.Xml;
using api;
using api.ClientEventHandlers;
using api.Models.Enums;
using api.ServerEvents;
using lib;
using Microsoft.VisualStudio.TestPlatform.Utilities;

namespace TestsBE;
[TestFixture]
[NonParallelizable]
public class ApiTests
{
    [SetUp]
    public async Task Setup()
    {
        Startup.Start(null);
    }

    [Test]
    public async Task ClientCanPublishNoteToSubject()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        await ws.DoAndAssert(new ClientWantsToSubscribeToSubjectDto()
        {
            subjectId = 1
        }, receivedMessages =>
        {
            return receivedMessages.Count(e => e.eventType == nameof(ServerSubscribesClientToSubject)) == 1;
        });
        
        Console.Write("passed 1");
        await ws.DoAndAssert(new ClientWantsToCreateNoteDto()
        {
            messageContent = "Test message",
            subjectId = 1
        }, receivedMessages =>
        {
            return receivedMessages.Count(e => e.eventType == nameof(ServerAddsNoteToSubject)) == 1;
        });
        
        ws.Client.Dispose();
    }
}
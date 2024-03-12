using System.Xml;
using api;
using api.ClientEventHandlers;
using api.Models.Enums;
using api.ServerEvents;
using lib;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using NUnit.Framework;

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
    public async Task ClientWantsToJournal()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();
        
        await ws.DoAndAssert(new ClientWantsToJournalDto() { username = "Journalist1", password = "password" },
            message =>
            {
                return message.Count(e => e.eventType == nameof(ServerAuthenticatesJournalist)) == 1;
            });
        
        ws.Client.Dispose();
    }
    
    
    [Test]
    public async Task ClientCanPublishNoteToSubject()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();
        
        await ws.DoAndAssert(new ClientWantsToOpenJournalDto()
        {
            username = "Journalist",
            password = "password"
        }, message =>
        {
            return message.Count(e => e.eventType == nameof(ServerAuthenticatesJournalist)) == 1;
        });

        await ws.DoAndAssert(new ClientWantsToSubscribeToSubjectDto()
        {
            subjectId = 1
        }, message =>
        {
            return message.Count(e => e.eventType == nameof(ServerSubscribesClientToSubject)) == 1;
        });
        
        // client wants to put a note in their subscribed subject
        await ws.DoAndAssert(new ClientWantsToCreateNoteDto()
            {
                messageContent = "test_message",
                subjectId = 1
            },
            message =>
            {
                return message.Count(e => e.eventType == nameof(ServerAddsNoteToSubject)) == 1;
            });
        
        
        ws.Client.Dispose();
    }

    
    
}
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
    
    [Test]
    public async Task ClientWantsToUpdateNote()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();
        
        await ws.DoAndAssert(new ClientWantsToJournalDto() { username = "Journalist2", password = "password" },
            message =>
            {
                return message.Count(e => e.eventType == nameof(ServerAuthenticatesJournalist)) == 1;
            });
        
        await ws.DoAndAssert(new ClientWantsToSubscribeToSubjectDto()
        {
            subjectId = 0
        }, message =>
        {
            return message.Count(e => e.eventType == nameof(ServerSubscribesClientToSubject)) == 1;
        });
        
        // client wants to put a note in their subscribed subject
        await ws.DoAndAssert(new ClientWantsToCreateNoteDto()
            {
                messageContent = "test_message",
                subjectId = 0
            },
            message =>
            {
                return message.Count(e => e.eventType == nameof(ServerAddsNoteToSubject)) == 1;
            });
        
        await ws.DoAndAssert(new ClientWantsToEditNoteDto() { id = 1, messageContent = "updated_message", subjectId = 0}, message => { return message.Count(e => e.eventType == nameof(ServerUpdatesNoteInSubject)) == 1; });
        
        ws.Client.Dispose();
    }
    
    [Test]
    public async Task ClientWantsToSubscribeToSubject()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();
        await ws.DoAndAssert(new ClientWantsToOpenJournalDto() { username = "Journalist", password = "password" }, message => { return message.Count(e => e.eventType == nameof(ServerAuthenticatesJournalist)) == 1; });
        await ws.DoAndAssert(new ClientWantsToSubscribeToSubjectDto() { subjectId = 1 }, message => { return message.Count(e => e.eventType == nameof(ServerSubscribesClientToSubject)) == 1; });
        ws.Client.Dispose();
    }
    
    [Test]
    public async Task ClientWantsToUnsubscribeFromSubject()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();
        await ws.DoAndAssert(new ClientWantsToOpenJournalDto() { username = "Journalist", password = "password" }, message => { return message.Count(e => e.eventType == nameof(ServerAuthenticatesJournalist)) == 1; });
        await ws.DoAndAssert(new ClientWantsToSubscribeToSubjectDto() { subjectId = 2 }, message => { return message.Count(e => e.eventType == nameof(ServerSubscribesClientToSubject)) == 1; });
        await ws.DoAndAssert(new ClientWantsToUnsubscribeFromSubjectDto() { subjectId = 2 }, message => { return message.Count(e => e.eventType == nameof(ServerUnsubscribesFromSubject)) == 1; });
        ws.Client.Dispose();
    }

    [Test]
    public async Task ClientWantsToDeleteNote()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();
        await ws.DoAndAssert(new ClientWantsToJournalDto() { username = "Journalist3", password = "password" }, message => { return message.Count(e => e.eventType == nameof(ServerAuthenticatesJournalist)) == 1; });
        await ws.DoAndAssert(new ClientWantsToSubscribeToSubjectDto() { subjectId = 2 }, message => { return message.Count(e => e.eventType == nameof(ServerSubscribesClientToSubject)) == 1; });
        await ws.DoAndAssert(new ClientWantsToCreateNoteDto() { messageContent = "test_message", subjectId = 2 }, message => { return message.Count(e => e.eventType == nameof(ServerAddsNoteToSubject)) == 1; });
        await ws.DoAndAssert(new ClientWantsToDeleteNoteDto() { id = 1, subjectId = 2 }, message => { return message.Count(e => e.eventType == nameof(ServerDeletesNoteInSubject)) == 1; });
        ws.Client.Dispose();
    }
    
    
}
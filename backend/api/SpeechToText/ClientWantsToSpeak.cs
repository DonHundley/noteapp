using System.ComponentModel.DataAnnotations;
using api.ClientEventHandlers;
using api.Exceptions;
using api.ServerEvents;
using api.State;
using Externalities;
using Externalities.ParameterModels;
using Externalities.QueryModels;
using Fleck;
using lib;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Serilog;

namespace api.SpeechToText;

public class ClientWantsToSpeakDto : BaseDto
{
   [Required] public byte[] AudioData { get; set; } 
   [Required] public int SubjectId { get; set; }
   
}

public class ClientWantsToSpeak(NoteRepository noteRepository) : BaseEventHandler<ClientWantsToSpeakDto>
{
    private static SpeechRecognizer recognizer;
    private static SpeechConfig config = SpeechConfig.FromSubscription(Environment.GetEnvironmentVariable("TTSKEY"), Environment.GetEnvironmentVariable("REGION"));

    private async Task<string> SpeechToText(byte[] audioData)
    {
        using var reader = new BinaryReader(new MemoryStream(audioData));
        using var audioInputStream = AudioInputStream.CreatePushStream();
        
        
        recognizer = new SpeechRecognizer(config, AudioConfig.FromStreamInput(audioInputStream));

        // Read the input audio as a stream and push it to the recognizer as needed
        byte[] audioBuffer = new byte[1024];
        int bytesRead;
        while ((bytesRead = reader.Read(audioBuffer, 0, audioBuffer.Length)) > 0)
        {
            audioInputStream.Write(audioBuffer, bytesRead);
        }
        // Signal that the stream is done
        audioInputStream.Close();

        var result = await recognizer.RecognizeOnceAsync();

        if (result.Reason == ResultReason.RecognizedSpeech)
        {
            return result.Text;
        }
        else if (result.Reason == ResultReason.NoMatch)
        {
            throw new Exception($"NOMATCH: No speech could be recognized.");
        }
        
        return "";
    }

    public override async Task Handle(ClientWantsToSpeakDto dto, IWebSocketConnection socket)
    {
        // check if we are using TTS
        if (Environment.GetEnvironmentVariable("NOTUSINGTTS") == "False")
        {
            var messageContent = await this.SpeechToText(dto.AudioData);

        
        
            var addNote = await noteRepository.Add(new CreateNoteParams { noteContent = messageContent, timestamp = DateTimeOffset.Now, subjectId = dto.SubjectId, sender = WebSocketStateService.GetClient(socket.ConnectionInfo.Id).Journalist.journalistId });
        
            var note = new Note
            {
                id = addNote.id,
                noteContent = addNote.noteContent,
                timestamp = DateTimeOffset.UtcNow,
                subjectId = addNote.subjectId,
                sender = WebSocketStateService.GetClient(socket.ConnectionInfo.Id).Journalist.journalistId
            };
         
        
            WebSocketStateService.AddNoteToSubject(note.subjectId,new ServerTranscribesNoteFromSpeech
            {
                note = note,
                subjectId = note.subjectId
            }); 
        }
        else
        {
            throw new Exception("TTS is currently disabled.");
        }
        
    }
}

public class ServerTranscribesNoteFromSpeech : BaseDto
{
    public Note? note { get; set; }
    public int subjectId { get; set; }
}
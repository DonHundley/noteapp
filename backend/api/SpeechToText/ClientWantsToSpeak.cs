using System.ComponentModel.DataAnnotations;
using System.Text;
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
    [Required]
    public string AudioData { get; set; } 

    [Required]
    public int SubjectId { get; set; }
}

public class ClientWantsToSpeak(NoteRepository noteRepository) : BaseEventHandler<ClientWantsToSpeakDto>
{

    private static SpeechRecognizer recognizer;
    private static SpeechConfig config = SpeechConfig.FromSubscription(Environment.GetEnvironmentVariable("TTSKEY"), Environment.GetEnvironmentVariable("REGION"));

    private byte[] ConvertFromBase64(String base64String) 
    {
        return Convert.FromBase64String(base64String);
    }

    private async Task<string> SpeechToText(string base64Audio)
    {
        
        
        var audioData = ConvertFromBase64(base64Audio);
    
        using var reader = new BinaryReader(new MemoryStream(audioData));
        using var audioInputStream = AudioInputStream.CreatePushStream(
            AudioStreamFormat.GetCompressedFormat(AudioStreamContainerFormat.OGG_OPUS));
        recognizer = new SpeechRecognizer(config, AudioConfig.FromStreamInput(audioInputStream));
    
        StringBuilder completeTranscript = new StringBuilder();
    
        recognizer.Recognized += (s, e) => {
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                completeTranscript.Append(e.Result.Text + " ");
            }
            else if (e.Result.Reason == ResultReason.NoMatch)
            {
                Console.WriteLine($"NOMATCH: Speech could not be recognized.");
            }
        };
    
        byte[] audioBuffer = new byte[1024];
        int bytesRead;
        while ((bytesRead = reader.Read(audioBuffer, 0, audioBuffer.Length)) > 0)
        {
            audioInputStream.Write(audioBuffer, bytesRead);
        }
    
        audioInputStream.Close();
    
        await recognizer.StartContinuousRecognitionAsync();
        
        // Assuming the audio content is less than 5 seconds long.
        await Task.Delay(TimeSpan.FromSeconds(5));
    
        await recognizer.StopContinuousRecognitionAsync();
    
        return completeTranscript.ToString().Trim();
    }

    public override async Task Handle(ClientWantsToSpeakDto dto, IWebSocketConnection socket)
    {
        // check if we are using TTS
        if (Environment.GetEnvironmentVariable("NOTUSINGTTS") == "False")
        {
            var messageContent = await SpeechToText(dto.AudioData);

        
        
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
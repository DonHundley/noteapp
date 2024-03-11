namespace Externalities.ParameterModels;

public class CreateNoteParams
{
    public string? noteContent { get; set; }
    public DateTimeOffset timestamp { get; set; }
    public int? subjectId { get; set; }
    public int sender { get; set; }
}


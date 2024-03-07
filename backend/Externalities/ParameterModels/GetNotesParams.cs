namespace Externalities.ParameterModels;


public class GetNotesParams(int subjectId, int lastNoteId = int.MaxValue)
{
    public int subjectId { get; set; } = subjectId;
    public int lastNoteId { get; private set; } = lastNoteId;
}
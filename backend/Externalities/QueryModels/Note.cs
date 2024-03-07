namespace Externalities.QueryModels;

public class Note
{
    public int id { get; set; }
    public string? noteContent { get; set; }
    public DateTimeOffset timestamp { get; set; }
    public int? subjectId { get; set; }
    public int sender { get; set; }
    
}
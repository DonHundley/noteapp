namespace Externalities.QueryModels;

public class Note
{
    public int Id { get; set; }
    public string? Content { get; set; }
    public string? Subject { get; set; }
    public DateTimeOffset timestamp { get; set; }
    
}
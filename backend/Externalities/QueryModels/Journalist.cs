namespace Externalities.QueryModels;

public class Journalist
{
    public int journalistId { get; set; }
    public string? username { get; set; }
    public string? hash { get; set; }
    public string? salt { get; set; }
}
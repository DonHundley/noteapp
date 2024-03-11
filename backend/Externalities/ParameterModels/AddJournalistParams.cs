namespace Externalities.ParameterModels;

public class AddJournalistParams(string username, string hash, string salt)
{
    public string username { get; set; } = username;
    public string hash { get; set; } = hash;
    public string salt { get; set; } = salt;
}
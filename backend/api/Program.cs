using Fleck;

var app = await ApiStartup.StartApi();
app.Run();

public static class ApiStartup
{
    public static async Task<WebApplication> StartApi()
    {
        EnvSetup.SetDefaultEnvVariables();

        Log.Information(JsonSerializer.Serialize(Environment.GetEnvironmentVariables(), new JsonSerializerOptions
        {
            WriteIndented = true
        }));

        var builder = WebApplication.CreateBuilder();

        
    }

    
}
using Serilog;
using System.Reflection;
using api.Exceptions;
using api.Helpers;
using api.Security;
using api.ServerEvents;
using api.State;
using Externalities;
using Fleck;
using lib;



namespace api;

public static class Startup
{

    public static void Main(string[] args)
    {
        var webApp = Start(args);
        webApp.Run();
    }
    
    public static WebApplication Start(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(
                outputTemplate: "\n{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}\n")
            .CreateLogger();


        

        var builder = WebApplication.CreateBuilder(args);
        
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? throw new Exception("Not found");
        if (string.IsNullOrEmpty(env))
            throw new Exception("Must have ASPNETCORE_ENVIRONMENT");
        
        string connectionString;
        
        if (env is "Development" or "Test")
        {
            connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException();
        }
        else
        {
            connectionString = Environment.GetEnvironmentVariable("MYSQLCONN") ?? throw new InvalidOperationException(); 
        }
        
        // create our auth singletons
        builder.Services.AddSingleton<TokenService>(_ => new TokenService());
        builder.Services.AddSingleton<CredentialService>(_ => new CredentialService());
        
        // create our repository singletons
        
        
        try
        {
            builder.Services.AddSingleton<RepositoryManagement>(_ => new RepositoryManagement(connectionString));
            builder.Services.AddSingleton<NoteRepository>(_ => new NoteRepository(connectionString));
            builder.Services.AddSingleton<JournalistRepository>(_ => new JournalistRepository(connectionString));
        }
        catch (Exception e)
        {
            throw new RepositoryGeneralException("Error connecting to database on build");
        }
        
        var services = builder.FindAndInjectClientEventHandlers(Assembly.GetExecutingAssembly());

        
        
        var app = builder.Build();
        var server = new WebSocketServer("ws://0.0.0.0:8181");
        
       
        
        
        // disable/enable text to speech
        var env1 = Environment.GetEnvironmentVariable("REGION");
        var env2 = Environment.GetEnvironmentVariable("TTSKEY");
        if (string.IsNullOrEmpty(env1) || string.IsNullOrEmpty(env2))
        {
            var notUsingTts = true;
            Environment.SetEnvironmentVariable("NOTUSINGTTS", notUsingTts.ToString());
        }
        else
        {
            var notUsingTts = false;
            Environment.SetEnvironmentVariable("NOTUSINGTTS", notUsingTts.ToString()); 
        }
        
        // if the environment is testing or it is explicitly stated, call the rebuild method in RepositoryManagement
        if (Environment.GetCommandLineArgs().Contains("--rebuild-db") || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Test")
        {
            var repositoryManagement = app.Services.GetRequiredService<RepositoryManagement>();
            repositoryManagement.ExecuteRebuildDb();
        }
        
        
        
        
        server.RestartAfterListenError = true;

        server.Start(socket =>
        {
            socket.OnOpen = () => WebSocketStateService.AddClient(socket.ConnectionInfo.Id, socket);
            socket.OnClose = () => WebSocketStateService.RemoveClient(socket.ConnectionInfo.Id);
            socket.OnMessage = async message =>
            {
                try
                {
                    await app.InvokeClientEventHandler(services, socket, message);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Global exception handler");
                    switch (e)
                    {
                        case InvalidEnumValueException:
                            socket.SendDto(new ServerSendsErrorMessageToClient()
                            {
                                errorMessage = "Invalid enum value, subject does not exist.",
                                receivedMessage = message
                            });
                            break;
                        case InvalidDatabaseSettingsException:
                            socket.SendDto(new ServerSendsErrorMessageToClient()
                            {
                                errorMessage = "There has been an issue communicating with the database.",
                                receivedMessage = message
                            });
                            break;
                        case SubException:
                            socket.SendDto(new ServerSendsErrorMessageToClient()
                            {
                                errorMessage = "You are not subscribed to the subject you are contributing to!",
                                receivedMessage = message
                            });
                            break;
                        case InvalidLoginException:
                            socket.SendDto(new ServerSendsErrorMessageToClient
                            {
                                errorMessage = "Invalid login credentials.",
                                receivedMessage = message
                            });
                            break;
                        default:
                            socket.SendDto(new ServerSendsErrorMessageToClient
                            {
                                // outside of intended scenarios, such as auth based errors,
                                // this shouldn't be thrown as I'm a flawless programmer and considered all possible exceptions. 
                                // but just in case, you never know. ¯\_(ツ)_/¯
                                errorMessage = e.Message,
                                receivedMessage = message
                            });
                            break;
                    }
                }
            };
        });

        return app;
    }
}



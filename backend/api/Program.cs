using Serilog;
using System.Reflection;
using api.Exceptions;
using api.Helpers;
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
        
        // create our repository singleton
        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException();
        builder.Services.AddSingleton<RepositoryManagement>(_ => new RepositoryManagement(connectionString));
        builder.Services.AddSingleton<NoteRepository>(_ => new NoteRepository(connectionString));
        builder.Services.AddSingleton<JournalistRepository>(_ => new JournalistRepository(connectionString));
        var services = builder.FindAndInjectClientEventHandlers(Assembly.GetExecutingAssembly());

        builder.WebHost.UseUrls("http://*:5142");
        var app = builder.Build();
        var port = Environment.GetEnvironmentVariable(ENV_VAR_KEYS.PORT.ToString()) ?? "8181";
        var server = new WebSocketServer("ws://0.0.0.0:8181");
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? throw new Exception("Not found");
        if (string.IsNullOrEmpty(env))
            throw new Exception("Must have ASPNETCORE_ENVIRONMENT");
        
        // if the environment is development, call the rebuild method in RepositoryManagement
        if (env == "Development")
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
                        case InvalidOperationException:
                            socket.SendDto(new ServerSendsErrorMessageToClient()
                            {
                                errorMessage = "There has been an issue communicating with the database.",
                                receivedMessage = message
                            });
                            break;
                        default:
                            socket.SendDto(new ServerSendsErrorMessageToClient
                            {
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



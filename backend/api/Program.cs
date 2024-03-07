using Serilog;
using System;
using System.Reflection;
using System.Text.Json;
using api;
using api.Helpers;
using api.ServerEvents;
using api.State;
using Fleck;
using lib;
using MySql.Data.MySqlClient;

var app = await ApiStartup.StartApi();
app.Run();

namespace api
{
    public static class ApiStartup
    {
        public static async Task<WebApplication> StartApi()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(
                    outputTemplate: "\n{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}\n")
                .CreateLogger();


            Log.Information(JsonSerializer.Serialize(Environment.GetEnvironmentVariables(), new JsonSerializerOptions
            {
                WriteIndented = true
            }));

            var builder = WebApplication.CreateBuilder();
            
            // create our repository singleton
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddSingleton<NoteRepository>(_ => new NoteRepository(connectionString!));

            var services = builder.FindAndInjectClientEventHandlers(Assembly.GetExecutingAssembly());
            
            var app = builder.Build();
            
            
            
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? throw new Exception("Not found");
            if (string.IsNullOrEmpty(env))
                throw new Exception("Must have ASPNETCORE_ENVIRONMENT");


            var server = new WebSocketServer("ws://0.0.0.0:8181");

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
                        Log.Error(e, "Global exeception handler");
                        socket.SendDto(new ServerSendsErrorMessageToClient
                        {
                            errorMessage = e.Message,
                            receivedMessage = message
                        });
                    }
                };
            });

            return app;
        }
    }   
}



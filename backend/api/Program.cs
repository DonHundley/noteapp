using Serilog;
using System;
using System.Text.Json;
using api;
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
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            
            builder.Services.AddTransient<MySqlConnection>(_ => new MySqlConnection(connectionString));

            var app = builder.Build();
            
            
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? throw new Exception("Not found");
            if (string.IsNullOrEmpty(env))
                throw new Exception("Must have ASPNETCORE_ENVIRONMENT");


            var server = new WebSocketServer("ws://0.0.0.0:8181");

            server.RestartAfterListenError = true;
            
            server.Start(socket =>
            {
                socket.OnOpen = () => { Console.WriteLine("Socket opened"); };
                socket.OnClose = () => { Console.WriteLine("Socket closed"); };
                socket.OnMessage = message => { Console.WriteLine($"Received message: {message}"); };
            });

            return app;
        }
    }   
}



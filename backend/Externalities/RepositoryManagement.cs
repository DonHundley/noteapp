using Dapper;
using MySql.Data.MySqlClient;

namespace Externalities;

public class RepositoryManagement(string connectionString)
{
    private MySqlConnection GetOpenConnection()
    {
        var connection = new MySqlConnection(connectionString);
        connection.Open();
        return connection;
    }
    
    public void ExecuteRebuildDb()
    {
        using (var connection = GetOpenConnection())
        {
            connection.Execute($@"
USE db;

DROP TABLE IF EXISTS notes;

DROP TABLE IF EXISTS journalist;

CREATE TABLE journalist (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(255) NOT NULL
);

CREATE TABLE notes (
    id INT AUTO_INCREMENT PRIMARY KEY,
    noteContent TEXT,
    sender INT,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    subjectId INT,
    FOREIGN KEY (sender) REFERENCES journalist(id)
);

INSERT INTO journalist (username) VALUES ('Bob');");
        }
    }
}
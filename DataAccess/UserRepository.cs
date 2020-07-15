using DataAccess.DTOs;
using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.IO;

namespace DataAccess
{
    public static class UserRepository
    {
        private static string db = @"lasterDB.db";

        private static SQLiteConnectionStringBuilder connectionStringBuilder;

        static UserRepository()
        {
            connectionStringBuilder = new SQLiteConnectionStringBuilder();

            connectionStringBuilder.DataSource = db;
        }

        public static void Insert(User user)
        {
            using (var connection = new SQLiteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText = $@"INSERT INTO [User]
([Login], [Password])
VALUES '{user.Login}', '{user.Password}');";

                command.ExecuteNonQuery();

            }
        }

        public static User GetUser(int id)
        {
            User user = null;
            try
            {
                using (var connection = new SQLiteConnection(connectionStringBuilder.ConnectionString))
                {
                    connection.Open();

                    var command = connection.CreateCommand();

                    command.CommandText = "SELECT * FROM User WHERE Id = $id";

                    command.Parameters.AddWithValue("$id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user.Id = (int)reader["Id"];

                            user.Login = (string)reader["Login"];

                            user.Password = (string)reader["Password"];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return user;
        }
        private const string CreateTableQuery = @"CREATE TABLE IF NOT EXISTS [MyTable] (
                                               [ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                               [Key] NVARCHAR(2048)  NULL,
                                               [Value] VARCHAR(2048)  NULL
                                               )";
        public static User GetUser(string login, string password)
        {
                    const string DatabaseFile = "databaseFile.db";
        const string DatabaseSource = "data source=" + DatabaseFile;

            // Create the file which will be hosting our database
            if (!File.Exists(DatabaseFile))
            {
                SQLiteConnection.CreateFile(DatabaseFile);
            }

            // Connect to the database 
            using (var connection = new SQLiteConnection(DatabaseSource))
            {
                // Create a database command
                using (var command = new SQLiteCommand(connection))
                {
                    connection.Open();

                    // Create the table
                    command.CommandText = CreateTableQuery;
                    command.ExecuteNonQuery();

                    // Insert entries in database table
                    command.CommandText = "INSERT INTO MyTable (Key,Value) VALUES ('key one','value one')";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO MyTable (Key,Value) VALUES ('key two','value two')";
                    command.ExecuteNonQuery();

                    // Select and display database entries
                    command.CommandText = "Select * FROM MyTable";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader["Key"] + " : " + reader["Value"]);
                        }
                    }
                    connection.Close(); // Close the connection to the database
                }
            }


            User user = null;
            try
            {
                using (var connection = new SQLiteConnection(connectionStringBuilder.ConnectionString))
                {
                    
                    var command = connection.CreateCommand();

                    command.Connection = connection;

                    command.CommandText = $@"SELECT * FROM User WHERE Login = '{login}' AND Password = '{password}'";

                    command.Connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user.Id = (int)reader["Id"];

                            user.Login = (string)reader["Login"];

                            user.Password = (string)reader["Password"];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return user;
        }

        public static List<User> GetAllUser()
        {
            List<User> user = new List<User>();
            try
            {
                using (var connection = new SQLiteConnection(connectionStringBuilder.ConnectionString))
                {
                    connection.Open();

                    var command = connection.CreateCommand();

                    command.CommandText = "SELECT * FROM User";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user.Add(new User
                            {
                                Id = (int)reader["Id"],
                                Login = (string)reader["Login"],
                                Password = (string)reader["Password"]
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return user;
        }
    }
}
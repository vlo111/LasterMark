using lasterMark.DataAccess.DTOs;
using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.IO;

namespace lasterMark.DataAccess
{
    public static class UserRepository
    {
        private static string db = @"laster.db";

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
VALUES ('{user.Login}', '{user.Password}');";

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
                            user.Id = (long)reader["Id"];

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

        public static User GetUser(string login, string password)
        {
            User user = null;

            try
            {
                using (var connection = new SQLiteConnection(connectionStringBuilder.ConnectionString))
                {
                    connection.Open();

                    var command = connection.CreateCommand();

                    command.Connection = connection;

                    command.CommandText = $@"SELECT * FROM User WHERE Login = '{login}' AND Password = '{password}'";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user = new User
                            {
                                Id = (long)reader["Id"],
                                Login = (string)reader["Login"],
                                Password = (string)reader["Password"]
                            };
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
                                Id = (long)reader["Id"],
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
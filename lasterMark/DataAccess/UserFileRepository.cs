using lasterMark.DataAccess.DTOs;
using System.Data.SQLite;
using System;

namespace lasterMark.DataAccess
{
    public class UserFileRepository
    {
        private static string db = @"LasterDB.db3";

        private static SQLiteConnectionStringBuilder connectionStringBuilder;

        static UserFileRepository()
        {
            connectionStringBuilder = new SQLiteConnectionStringBuilder();

            connectionStringBuilder.DataSource = db;
        }

        public void Insert(UserFiles userFiles)
        {
            using (var connection = new SQLiteConnection(db))
            {
                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText = $@"INSERT INTO [UserFiles]
           ([UserId]
           ,[BackgroundImageData]
           ,[EzdImageData]
           ,[ReadyMadeImageData])
VALUES {userFiles.UserId}
, '{userFiles.BackgroundImageData}'
, '{userFiles.EzdImageData}'
, '{userFiles.ReadyMadeImageData}');";

                command.ExecuteNonQuery();

            }
        }

        public UserFiles GetUserFiles(long id)
        {
            UserFiles userFiles = null;
            try
            {
                using (var connection = new SQLiteConnection(db))
                {
                    connection.Open();

                    var command = connection.CreateCommand();

                    command.CommandText = "SELECT * FROM UserFiles WHERE Id = $id";

                    command.Parameters.AddWithValue("$id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userFiles.Id = (long)reader["Id"];

                            userFiles.UserId = (long)reader["UserId"];

                            userFiles.BackgroundImageData = (byte[])reader["BackgroundImageData"];

                            userFiles.EzdImageData = (byte[])reader["EzdImageData"];

                            userFiles.ReadyMadeImageData = (byte[])reader["ReadyMadeImageData"];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return userFiles;
        }
    }
}

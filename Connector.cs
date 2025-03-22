using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.ComponentModel.Design;
using System.IO;
using System.Xml;
namespace ADO.NET
{
    static class Connector
    {
        const int PADDING = 30;
        const string CONNECTION_STRING = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        static readonly SqlConnection connection;
        static Connector()
        {
            connection = new SqlConnection(CONNECTION_STRING);
            //статический конструктор нужен только для инициализации статических полей класса.
        }
        public static void SelectDirectors()
        {
            Select("*", "Directors");
        }
        public static void SelectMovies()
        {
            Connector.Select("title,release_date,FORMATMESSAGE(N'%s %s',first_name,last_name)", "Movies, Directors", "director=director_id");
        }
        public static void Select(string colums, string tables, string condition=null)
        {
            //3) Создаем команду, которую нужно выполнить на сервере:
            //string cmd = "SELECT title,release_date,FORMATMESSAGE(N'%s %s',first_name,last_name) FROM Movies,Directors WHERE director=director_id";
            string cmd = $"SELECT {colums} FROM {tables}";
            if (condition != null) cmd += $" WHERE {condition}";
            cmd += ";";

            SqlCommand command = new SqlCommand(cmd, connection);

            //4) Получаем результаты выполнения команды:
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            //5) Обрабатываем результаты запроса:
            if (reader.HasRows)
            {
                Console.WriteLine("===================================================================================");
                for (int i = 0; i < reader.FieldCount; i++)
                    Console.Write(reader.GetName(i).PadRight(PADDING));
                Console.WriteLine();
                Console.WriteLine("===================================================================================");
                while (reader.Read())
                {
                    //Console.WriteLine($"{reader[0].ToString().PadRight(5)}{reader[2].ToString().PadRight(15)}{reader[1].ToString().PadRight(15)}");
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write(reader[i].ToString().PadRight(PADDING));
                    }
                    Console.WriteLine();
                }
            }

            //6) Закрываем Reader и Connection:
            reader.Close();
            connection.Close();
        }

        public static void InsertMovie(string title, string release_date, int director)
        {
            string cmd1 = $"SELECT COUNT(movie_id) FROM Movies";
            SqlCommand command1 = new SqlCommand(cmd1, connection);
            connection.Open();
            int quantity = ((int)command1.ExecuteScalar())+1;//самое последнее id

            string cmd = $"INSERT Movies (movie_id, title, release_date, director) VALUES (N'{quantity}', N'{title}', N'{release_date}', N'{director}')";
            SqlCommand command = new SqlCommand(cmd, connection);
            
            command.ExecuteNonQuery();
            connection.Close();
        }

        public static void Insert()
        {
            string nameOfDB;
            Console.WriteLine("Введите таблицу, в которую необходимо добавить данные:");
            nameOfDB = Console.ReadLine();
            List<string> columnNames = new List<string>(); // названия столбцов
            List<string> valuesTable = new List<string>();
            connection.Open();

            using (SqlCommand command = new SqlCommand($"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @nameOfDB", connection))
            {
                command.Parameters.Add(new SqlParameter("@nameOfDB", nameOfDB));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        columnNames.Add(reader.GetString(0)); // получаем название столбцов
                    }
                }
            }

            foreach (string columnName in columnNames)
            {
                Console.WriteLine($"Введите {columnName}:");
                valuesTable.Add(Console.ReadLine());
            }

            string cmd = $"INSERT INTO {nameOfDB} ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", columnNames.Select((_, i) => $"@p{i}"))});";

            using (SqlCommand commandInsert = new SqlCommand(cmd, connection))
            {
                for (int i = 0; i < valuesTable.Count; i++)
                {
                    commandInsert.Parameters.AddWithValue($"@p{i}", valuesTable[i]);
                }
                commandInsert.ExecuteNonQuery();
            }
            connection.Close();
        }
        public static void InsertDirector(string first_name, string last_name)
        {
            string cmd = $"INSERT Directors(first_name,last_name) VALUES (N'{first_name}', N'{last_name}')";
            SqlCommand command = new SqlCommand(cmd, connection);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}

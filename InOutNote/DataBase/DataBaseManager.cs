using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InOutNote.DataBase
{
    public class DataBaseManager
    {
        private string filePath = Directory.GetCurrentDirectory() + @"\Account.db";
        private static DataBaseManager instance = new DataBaseManager();

        public static DataBaseManager Instance
        {
            get
            {
                if (instance == null) instance = new DataBaseManager();
                return instance;
            }
        }

        public DataBaseManager()
        {
            
            
        }

        internal void CreateDB()
        {
            if (File.Exists(filePath)) { }
            else
            {
                SQLiteConnection.CreateFile(filePath);

                string path = String.Format("Data Source = {0}", filePath);

                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();
                    string sql = "CREATE TABLE IF NOT EXISTS Balance_Info ( " +
                        "InOut TEXT NOT NULL, " +
                        "Money INTEGER NOT NULL, " +
                        "UseDate INTEGER, " +
                        "Bank  INTEGER, " +
                        "Card  INTEGER, " +
                        "UseWhere  INTEGER NOT NULL);";

                    string sql2 = "CREATE TABLE IF NOT EXISTS Bank_Code (" +
                        "Bank INTEGER NOT NULL, " +
                        "Code TEXT NOT NULL, " +
                        "Kind TEXT, " +
                        "PRIMARY KEY(Bank) );";

                    string sql3 = "CREATE TABLE Card_Code(" +
                        "Card  INTEGER NOT NULL, " +
                        "Code TEXT NOT NULL, " +
                        "PRIMARY KEY(Card) );";

                    string sql4 = "CREATE TABLE Use_Code(" +
                        "Use INTEGER NOT NULL, " +
                        "Code TEXT NOT NULL, " +
                        "PRIMARY KEY(Use) );";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    command.ExecuteNonQuery();

                    command = new SQLiteCommand(sql2, connection);
                    command.ExecuteNonQuery();

                    command = new SQLiteCommand(sql3, connection);
                    command.ExecuteNonQuery();

                    command = new SQLiteCommand(sql4, connection);
                    command.ExecuteNonQuery();

                    Console.WriteLine("Create DB Success");
                }
            }
        }
        internal List<String> GetWeeklyData()
        {
            List<String> data = new List<String>();

            string path = String.Format("Data Source = {0}", filePath);

            using (SQLiteConnection connection = new SQLiteConnection(path))
            {
                connection.Open();

                //string query = "";
            }

            return data;
        }
    }
}

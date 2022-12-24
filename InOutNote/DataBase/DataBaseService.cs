using InOutNote.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InOutNote.DataBase
{
    public class DataBaseService : IDataBaseService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DataBaseService));

        private string filePath = Directory.GetCurrentDirectory() + @"\Account.db";

        private static DataBaseService instance = new DataBaseService();

        public static DataBaseService Instance
        {
            get 
            {
                if (instance == null) instance = new DataBaseService();
                return instance;
            }

        }
        public void CreateDB()
        {
            if (File.Exists(filePath)) { log.Info($"{MethodBase.GetCurrentMethod()?.Name}::DB already exists"); }
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

                    log.Info($"{MethodBase.GetCurrentMethod()?.Name}::Create DB Success");
                }
            }
        }

        public List<InOutModel> GetWeeklyInOutData()
        {
            List<InOutModel> returnData = new List<InOutModel>();

            string path = String.Format("Data Source = {0}", filePath);
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string weekBefore = DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd");
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();

                    string sql = "SELECT InOut, SUM(Money) AS Money, UseDate " +
                    "FROM Balance_Info " +
                    $"WHERE UseDate BETWEEN '{weekBefore}' AND '{today}' " +
                    $"GROUP BY InOut, UseDate " +
                    $"ORDER BY UseDate;";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        log.Info(reader["InOut"].ToString());
                        log.Info(reader["Money"].ToString());
                        log.Info(reader["UseDate"].ToString());

                        returnData.Add(new InOutModel
                        {
                            InOut = reader["InOut"].ToString() == "IN" ? "입금" : "출금",
                            Money = (int)(long)reader["Money"],
                            UseDate = reader["UseDate"].ToString(),
                        });
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error($"{MethodBase.GetCurrentMethod()?.Name}::{ex.Message}");
            }
            return returnData;
        }

        public List<InOutModel> GetMonthlyInOutData()
        {
            List<InOutModel> returnData = new List<InOutModel>();

            string path = String.Format("Data Source = {0}", filePath);

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();

                    string sql = "SELECT InOut, SUM(Money) AS Money, UseDate " +
                    "FROM Balance_Info " +
                    $"GROUP BY InOut, strftime(\"%m\", UseDate)  " +
                    $"ORDER BY UseDate;";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        log.Info(reader["InOut"].ToString());
                        log.Info(reader["Money"].ToString());
                        log.Info(reader["UseDate"].ToString());

                        returnData.Add(new InOutModel
                        {
                            InOut = reader["InOut"].ToString() == "IN" ? "입금" : "출금",
                            Money = (int)(long)reader["Money"],
                            UseDate = reader["UseDate"].ToString(),
                        });
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error($"{MethodBase.GetCurrentMethod()?.Name}::{ex.Message}");
            }
            return returnData;
        }
    }
}

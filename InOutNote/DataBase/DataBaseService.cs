using InOutNote.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

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
                        "UseWhere  INTEGER NOT NULL, " +
                        "Detail TEXT);";

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

        public List<InOutModel> SelectWeeklyInOutData()
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
                            Money = ((long)reader["Money"]).ToString(),
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
        public List<InOutModel> SelectMonthlyInOutData()
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
                            Money = ((long)reader["Money"]).ToString(),
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
        public List<InOutModel> SelectAllInOutData(string fromDate, string ToDate, InOutModel inOutModel)
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

                    string sql = "SELECT InOut, Money, A.Code AS Bank, B.Code AS Card, C.Code AS UseWhere, A.Kind, UseDate, Detail " +
                        "FROM Balance_Info " +
                        "LEFT JOIN Bank_Code A ON Balance_Info.Bank = A.Bank " +
                        "LEFT JOIN Card_Code B ON Balance_Info.Card = B.Card " +
                        "LEFT JOIN Use_Code C ON Balance_Info.UseWhere = C.Use " +
                        $"WHERE UseDate BETWEEN '{fromDate}' AND '{ToDate}' ";

                    if (inOutModel.InOut != "전체") sql = sql + $"AND InOut = '{(inOutModel.InOut == "입금" ? "IN" : "OUT")}' ";
                    if (inOutModel.Kind != "전체") sql = sql + $"AND A.Kind = '{inOutModel.Kind}' ";
                    if (inOutModel.Bank != "전체") sql = sql + $"AND A.Code = '{inOutModel.Bank}' ";
                    if (inOutModel.Card != "전체") sql = sql + $"AND B.Code = '{inOutModel.Card}' ";
                    if (inOutModel.Use != "전체") sql = sql + $"AND C.Code = '{inOutModel.Use}' ";

                    sql = sql + $"ORDER BY UseDate;";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        log.Info(reader["InOut"].ToString());
                        log.Info(reader["Money"].ToString());
                        log.Info(reader["UseDate"].ToString());
                        log.Info(reader["Kind"].ToString());
                        log.Info(reader["Bank"].ToString());
                        log.Info(reader["Card"].ToString());
                        log.Info(reader["UseWhere"].ToString());
                        log.Info(reader["Detail"].ToString());

                        returnData.Add(new InOutModel
                        {
                            InOut = reader["InOut"].ToString() == "IN" ? "입금" : "출금",
                            Money = ((long)reader["Money"]).ToString(),
                            UseDate = reader["UseDate"].ToString(),
                            Kind = reader["Kind"].ToString(),
                            Bank = reader["Bank"].ToString(),
                            Card = reader["Card"].ToString(),
                            Use = reader["UseWhere"].ToString(),
                            Detail = reader["Detail"].ToString()
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
        public List<string> SelectBankCode()
        {
            List<string> returnData = new List<string>();

            string path = String.Format("Data Source = {0}", filePath);

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();

                    string sql = "SELECT Code " +
                    "FROM Bank_Code;";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        log.Info(reader["Code"].ToString());

                        returnData.Add(reader["Code"]?.ToString()!);
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
        public List<string> SelectCardCode()
        {
            List<string> returnData = new List<string>();

            string path = String.Format("Data Source = {0}", filePath);

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();

                    string sql = "SELECT Code " +
                    "FROM Card_Code;";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        log.Info(reader["Code"].ToString());

                        returnData.Add(reader["Code"]?.ToString()!);
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
        public List<string> SelectUseCode()
        {
            List<string> returnData = new List<string>();

            string path = String.Format("Data Source = {0}", filePath);

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();

                    string sql = "SELECT Code " +
                    "FROM Use_Code;";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        log.Info(reader["Code"].ToString());

                        returnData.Add(reader["Code"]?.ToString()!);
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

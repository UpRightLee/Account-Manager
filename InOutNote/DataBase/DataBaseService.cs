using InOutNote.Models;
using log4net;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

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
                        "UseDate TEXT, " +
                        "Bank  INTEGER, " +
                        "Card  INTEGER, " +
                        "UseWhere  INTEGER NOT NULL, " +
                        "Detail TEXT);";

                    string sql2 = "CREATE TABLE IF NOT EXISTS Bank_Code (" +
                        "Bank INTEGER NOT NULL, " +
                        "Description TEXT NOT NULL, " +
                        "Kind TEXT, " +
                        "PRIMARY KEY(Bank) );";

                    string sql3 = "CREATE TABLE IF NOT EXISTS Card_Code(" +
                        "Card  INTEGER NOT NULL, " +
                        "Description TEXT NOT NULL, " +
                        "Bank INTEGER NOT NULL," +
                        "PRIMARY KEY(Card) " +
                        "FOREIGN KEY(Bank) REFERENCES Bank_Code(Bank));";

                    string sql4 = "CREATE TABLE IF NOT EXISTS Use_Code(" +
                        "Use INTEGER NOT NULL, " +
                        "Description TEXT NOT NULL, " +
                        "PRIMARY KEY(Use) );";

                    string sql5 = "CREATE TABLE IF NOT EXISTS Credit_Card_Use_Info(" +
                        "Money INTEGER NOT NULL, " +
                        "UseDate TEXT, " +
                        "Bank INTEGER, " +
                        "Card INTEGER, " +
                        "UseWhere INTEGER NOT NULL, " +
                        "Detail TEXT)";

                    string sql6 = "CREATE TABLE IF NOT EXISTS Bank_Card_Use_Set(" +
                        "Bank INTEGER NOT NULL, " +
                        "Card INTEGER, " +
                        "Use INTEGER NOT NULL, " +
                        "Kind NOT NULL)";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    command.ExecuteNonQuery();

                    command = new SQLiteCommand(sql2, connection);
                    command.ExecuteNonQuery();

                    command = new SQLiteCommand(sql3, connection);
                    command.ExecuteNonQuery();

                    command = new SQLiteCommand(sql4, connection);
                    command.ExecuteNonQuery();

                    command = new SQLiteCommand(sql5, connection);
                    command.ExecuteNonQuery();

                    command = new SQLiteCommand(sql6, connection);
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
                    $"AND UseWhere != 19 " +
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

            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string monthBefore = DateTime.Now.AddMonths(-6).ToString("yyyy-MM") + "-01";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();

                    string sql = "SELECT InOut, SUM(Money) AS Money, UseDate " +
                    "FROM Balance_Info " +
                    "WHERE UseWhere != 19 " +
                    $"AND UseDate BETWEEN '{monthBefore}' AND '{today}' " +
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
        public List<InOutModel> SelectCreditCardData(string fromDate, string ToDate)
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

                    string sql = "SELECT Money, A.Description AS Bank, B.Description AS Card, C.Description AS UseWhere, A.Kind, UseDate, Detail " +
                        "FROM Credit_Card_Use_Info " +
                        "LEFT JOIN Bank_Code A ON Credit_Card_Use_Info.Bank = A.Bank " +
                        "LEFT JOIN Card_Code B ON Credit_Card_Use_Info.Card = B.Card " +
                        "LEFT JOIN Use_Code C ON Credit_Card_Use_Info.UseWhere = C.Use " +
                        $"WHERE UseDate BETWEEN '{fromDate}' AND '{ToDate}' " +
                        $"ORDER BY UseDate DESC;";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        log.Info(reader["Money"].ToString());
                        log.Info(reader["UseDate"].ToString());
                        log.Info(reader["Kind"].ToString());
                        log.Info(reader["Bank"].ToString());
                        log.Info(reader["Card"].ToString());
                        log.Info(reader["UseWhere"].ToString());
                        log.Info(reader["Detail"].ToString());

                        returnData.Add(new InOutModel
                        {
                            InOut = "출금",
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

                    string sql = "SELECT InOut, Money, A.Description AS Bank, B.Description AS Card, C.Description AS UseWhere, A.Kind, UseDate, Detail " +
                        "FROM Balance_Info " +
                        "LEFT JOIN Bank_Code A ON Balance_Info.Bank = A.Bank " +
                        "LEFT JOIN Card_Code B ON Balance_Info.Card = B.Card " +
                        "LEFT JOIN Use_Code C ON Balance_Info.UseWhere = C.Use " +
                        $"WHERE UseDate BETWEEN '{fromDate}' AND '{ToDate}' ";

                    if (inOutModel.InOut != "전체") sql = sql + $"AND InOut = '{(inOutModel.InOut == "입금" ? "IN" : "OUT")}' ";
                    if (inOutModel.Kind != "전체") sql = sql + $"AND A.Kind = '{inOutModel.Kind}' ";
                    if (inOutModel.Bank != "전체") sql = sql + $"AND A.Description = '{inOutModel.Bank}' ";
                    if (inOutModel.Card != "전체") sql = sql + $"AND B.Description = '{inOutModel.Card}' ";
                    if (inOutModel.Use != "전체") sql = sql + $"AND C.Description = '{inOutModel.Use}' ";

                    sql = sql + $"ORDER BY UseDate DESC;";

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
        public List<Bank> SelectBankCode()
        {
            List<Bank> returnData = new List<Bank>();

            string path = String.Format("Data Source = {0}", filePath);

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();

                    string sql = "SELECT A.Bank, A.Description, B.Description AS Card, A.Kind " +
                        "FROM Bank_Code A " +
                        "LEFT JOIN Card_Code B ON A.Bank = B.Bank; ";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        log.Info(reader["Bank"].ToString());
                        log.Info(reader["Description"].ToString());
                        log.Info(reader["Kind"].ToString());
                        log.Info(reader["Card"].ToString());

                        returnData.Add( new Bank
                        {
                            Name = ((long)reader["Bank"]).ToString(),
                            Description = reader["Description"].ToString(),
                            Kind = reader["Kind"].ToString(),
                            Card = reader["Card"].ToString()
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
        public List<Card> SelectCardCode()
        {
            List<Card> returnData = new List<Card>();

            string path = String.Format("Data Source = {0}", filePath);

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();

                    string sql = "SELECT Card, Description, Bank " +
                    "FROM Card_Code;";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        log.Info(reader["Card"].ToString());
                        log.Info(reader["Description"].ToString());
                        log.Info(reader["Bank"].ToString());
                        returnData.Add( new Card
                        {
                            Name = ((long)reader["Card"]).ToString(),
                            Description = reader["Description"]?.ToString()!,
                            Bank = ((long)reader["Bank"]).ToString()
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
        public List<Use> SelectUseCode()
        {
            List<Use> returnData = new List<Use>();

            string path = String.Format("Data Source = {0}", filePath);

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();

                    string sql = "SELECT Use, Description " +
                    "FROM Use_Code;";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        log.Info(reader["Use"].ToString());
                        log.Info(reader["Description"].ToString());

                        returnData.Add( new Use
                        {
                            Name = ((long)reader["Use"]).ToString(),
                            Description = reader["Description"]?.ToString()!
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
        public List<Bank> SelectBankCardCode(Bank bank)
        {
            List<Bank> returnData = new List<Bank>();

            string path = String.Format("Data Source = {0}", filePath);

            try
            {
                string inCard = bank.Card!;
                string inKind = bank.Kind!;
                string inDescription = bank.Description!;

                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();
                    string sql = "SELECT A.Bank, A.Description, B.Description AS Card, A.Kind " +
                        "FROM Bank_Code A " +
                        "LEFT JOIN Card_Code B ON A.Bank = B.Bank ";
                    if (bank.Description == "전체") {}
                    else
                    {
                        sql += $"WHERE A.Description = '{bank.Description}' ";
                    }

                    if (bank.Kind == "전체") sql += ";";
                    else
                    {
                        if (bank.Description == "전체") sql += $"WHERE A.Kind = '{bank.Kind}';";
                        else sql += $"AND A.Kind = '{bank.Kind}';";
                    }

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        log.Info(reader["Bank"].ToString());
                        log.Info(reader["Description"].ToString());
                        log.Info(reader["Kind"].ToString());
                        log.Info(reader["Card"].ToString());

                        returnData.Add(new Bank
                        {
                            Name = ((long)reader["Bank"]).ToString(),
                            Description = reader["Description"].ToString(),
                            Kind = reader["Kind"].ToString(),
                            Card = reader["Card"].ToString()
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
        public List<SummaryData> SelectBalanceInfo(string year)
        {
            List<SummaryData> returnData = new List<SummaryData>();

            string path = String.Format("Data Source = {0}", filePath);

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();

                    string sql = "SELECT SUM(Money) AS MONEY, strftime('%m', UseDate) AS Month, InOut " +
                        "FROM Balance_Info " +
                        $"WHERE strftime('%Y', UseDate) = '{year}' " +
                        "GROUP BY InOut, strftime('%m', UseDate) " +
                        "ORDER BY UseDate ;";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        log.Info(reader["InOut"].ToString());
                        log.Info(reader["Money"].ToString());
                        log.Info(reader["Month"].ToString());

                        returnData.Add(new SummaryData
                        {
                            InOut = reader["InOut"].ToString() == "IN" ? "입금" : "출금",
                            Money = ((long)reader["Money"]).ToString(),
                            Month = reader["Month"].ToString(),
                            Year = year
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
        public List<SummaryData> SelectBankSummaryList()
        {
            List<SummaryData> returnData = new List<SummaryData>();

            string path = String.Format("Data Source = {0}", filePath);

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();

                    string sql = "SELECT SUM(money) AS Money, B.Description, InOut " +
                        "FROM Balance_Info A " +
                        "LEFT JOIN Bank_Code B " +
                        "ON A.Bank = B.Bank " +
                        "Group By B.Description, InOut; ";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        log.Info(reader["InOut"].ToString());
                        log.Info(reader["Description"].ToString());
                        log.Info(reader["Money"].ToString());

                        returnData.Add(new SummaryData
                        {
                            InOut = reader["InOut"].ToString() == "IN" ? "입금" : "출금",
                            Money = ((long)reader["Money"]).ToString(),
                            Bank = reader["Description"].ToString()
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
        public List<BankCardUseSet> SelectBankCardUseSetList()
        {
            List<BankCardUseSet> returnData = new List<BankCardUseSet>();

            string path = String.Format("Data Source = {0}", filePath);

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();

                    string sql = "SELECT M.Kind, A.Description AS Bank, B.Description AS Card, C.Description AS Use " +
                        "FROM Bank_Card_Use_Set M " +
                        "LEFT JOIN Bank_Code A ON M.Bank = A.Bank " +
                        "LEFT JOIN Card_Code B ON M.Card = B.Card " +
                        "LEFT JOIN Use_Code C ON M.Use = C.Use; ";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        returnData.Add(new BankCardUseSet
                        {
                           KindName = reader["Kind"].ToString(),
                           BankName = reader["Bank"].ToString(),
                           CardName = reader["Card"].ToString(),
                           UseName = reader["Use"].ToString(),
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
        public string SelectINBalanceInfo()
        {
            string returnData = "";

            string path = String.Format("Data Source = {0}", filePath);

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();

                    string sql = "SELECT SUM(Money) AS MONEY " +
                        "FROM Balance_Info " +
                        $"WHERE InOut = 'IN';";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        log.Info(reader["Money"].ToString());

                        returnData = ((long)reader["Money"]).ToString();
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
        public string SelectOUTBalanceInfo()
        {
            string returnData = "";

            string path = String.Format("Data Source = {0}", filePath);

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();

                    string sql = "SELECT SUM(Money) AS MONEY " +
                        "FROM Balance_Info " +
                        $"WHERE InOut = 'OUT';";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        log.Info(reader["Money"].ToString());

                        returnData = ((long)reader["Money"]).ToString();
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

        public bool DeleteInOutData(InOutModel inOutData)
        {
            bool returnData = false;
            string path = String.Format("Data Source = {0}", filePath);

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();
                    string sql = "";

                    if (inOutData.Kind == "신용" && inOutData.Use != "신용카드값")
                    {
                        sql = "DELETE FROM Credit_Card_Use_Info " +
                        "WHERE " +
                        $"Bank = (SELECT Bank FROM Bank_Code WHERE Description = '{inOutData.Bank}' AND Kind = '{inOutData.Kind}') " +
                        $"AND Money = {inOutData.Money!.Replace(",", "")} " +
                        $"AND UseDate = '{inOutData.UseDate}' " +
                        $"AND UseWhere = (SELECT Use FROM Use_Code WHERE Description = '{inOutData.Use}');";
                    }
                    else
                    {
                        sql = "DELETE FROM Balance_Info " +
                        "WHERE " +
                        $"Bank = (SELECT Bank FROM Bank_Code WHERE Description = '{inOutData.Bank}' AND Kind = '{inOutData.Kind}') " +
                        $"AND InOut = '{(inOutData.InOut == "입금" ? "IN" : "OUT")}' " +
                        $"AND Money = {inOutData.Money!.Replace(",", "")} " +
                        $"AND UseDate = '{inOutData.UseDate}' " +
                        $"AND UseWhere = (SELECT Use FROM Use_Code WHERE Description = '{inOutData.Use}');";
                    }
                    

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    returnData = command.ExecuteNonQuery() > 0;
                }
                if (returnData) return true;
                else return false;
            }
            catch (Exception ex)
            {
                log.Error($"{MethodBase.GetCurrentMethod()?.Name}::{ex.Message}");
                return false;
            }
            
        }
        public bool DeleteBankCode(Bank bank)
        {
            bool returnData = false;
            bool returnData2 = false;
            string path = String.Format("Data Source = {0}", filePath);

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();

                    if (bank.Kind == "자동이체")
                    {
                        string sql = "DELETE FROM Bank_Code " +
                        "WHERE " +
                        $"Description = '{bank.Description}' " +
                        $"AND Kind = '{bank.Kind}';";

                        SQLiteCommand command = new SQLiteCommand(sql, connection);
                        returnData = command.ExecuteNonQuery() > 0;
                        returnData2 = true;
                    }
                    else
                    {
                        string sql = "DELETE FROM Card_Code " +
                        "WHERE " +
                        $"Description = '{bank.Card}';";

                        string sql2 = "DELETE FROM Bank_Code " +
                        "WHERE " +
                        $"Description = '{bank.Description}' " +
                        $"AND Kind = '{bank.Kind}';";

                        SQLiteCommand command = new SQLiteCommand(sql, connection);
                        SQLiteCommand command2 = new SQLiteCommand(sql2, connection);

                        returnData = command.ExecuteNonQuery() > 0;
                        returnData2 = command2.ExecuteNonQuery() > 0;
                    }
                    
                }
                if (returnData && returnData2) return true;
                else return false;
            }
            catch (Exception ex)
            {
                log.Error($"{MethodBase.GetCurrentMethod()?.Name}::{ex.Message}");
                return false;
            }
        }
        public bool DeleteUseCode(Use use)
        {
            bool returnData = false;
            string path = String.Format("Data Source = {0}", filePath);

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();

                    string sql = "DELETE FROM Use_Code " +
                        "WHERE " +
                        $"Description = '{use.Description}';";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    returnData = command.ExecuteNonQuery() > 0;
                }
                if (returnData) return true;
                else return false;
            }
            catch (Exception ex)
            {
                log.Error($"{MethodBase.GetCurrentMethod()?.Name}::{ex.Message}");
                return false;
            }
        }
        public bool DeleteCreditData(string month)
        {
            bool returnData = false;
            string path = String.Format("Data Source = {0}", filePath);

            string year = "";

            if (DateTime.Now.Month == 1 && DateTime.Now.Day > 14) year = DateTime.Today.AddYears(-1).Year.ToString();
            else year = DateTime.Now.Year.ToString();

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();

                    string sql = "DELETE FROM Credit_Card_Use_Info " +
                        $"WHERE strftime('%Y-%m', UseDate) = '{year}-{month}';";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    returnData = command.ExecuteNonQuery() > 0;
                }
                if (returnData) return true;
                else return false;
            }
            catch (Exception ex)
            {
                log.Error($"{MethodBase.GetCurrentMethod()?.Name}::{ex.Message}");
                return false;
            }
        }
        public bool DeleteBankCardUseSet(string bank, string card, string use)
        {
            bool returnData = false;
            string path = String.Format("Data Source = {0}", filePath);

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();

                    string sql = "DELETE FROM Bank_Card_Use_Set " +
                        "WHERE " +
                        $"Bank = (SELECT Bank From Bank_Code WHERE Description = '{bank}') " +
                        $"AND Use = (SELECT Use From Use_Code WHERE Description = '{use}')";

                    if (card != "")
                    {
                        sql += $"AND Card = (SELECT Card From Card_Code WHERE Description = '{card}');";
                    }
                    else sql += ";";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    returnData = command.ExecuteNonQuery() > 0;
                }
                if (returnData) return true;
                else return false;
            }
            catch (Exception ex)
            {
                log.Error($"{MethodBase.GetCurrentMethod()?.Name}::{ex.Message}");
                return false;
            }
        }

        public bool InsertInOutData(InOutModel inOutData)
        {
            bool returnData = false;
            string path = String.Format("Data Source = {0}", filePath);

            string inOut = inOutData.InOut == "출금" ? "OUT" : "IN";
            string money = inOutData.Money!;
            string bank = inOutData.Bank!;
            string useDate = inOutData.UseDate!;
            string card = inOutData.Card!;
            string use = inOutData.Use!;
            string detail = inOutData.Detail!;
            string kind = inOutData.Kind!;
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();
                    string sql = "";

                    if (kind == "신용" && use != "신용카드값")
                    {
                        sql = "INSERT INTO Credit_Card_Use_Info " +
                            " (Money, UseDate, Bank, Card, UseWhere, Detail ) " +
                            "VALUES " +
                            $"({money}, " +
                            $"'{useDate}', " +
                            $"(SELECT Bank FROM Bank_Code WHERE Bank_Code.Description = '{bank}' AND Bank_Code.Bank = (SELECT Bank FROM Card_Code WHERE Card_Code.Description = '{card}')), " +
                            $"(SELECT Card FROM Card_Code WHERE Card_Code.Description = '{card}'), " +
                            $"(SELECT Use FROM Use_Code WHERE Use_Code.Description = '{use}'), " +
                            $"'{detail}'); ";
                    }
                    else
                    {
                        if (card != null)
                        {
                            sql = "INSERT INTO Balance_Info " +
                            " (InOut, Money, UseDate, Bank, Card, UseWhere, Detail ) " +
                            "VALUES " +
                            $"('{inOut}', " +
                            $"{money}, " +
                            $"'{useDate}', " +
                            $"(SELECT Bank FROM Bank_Code WHERE Bank_Code.Description = '{bank}' AND Bank_Code.Bank = (SELECT Bank FROM Card_Code WHERE Card_Code.Description = '{card}')), " +
                            $"(SELECT Card FROM Card_Code WHERE Card_Code.Description = '{card}'), " +
                            $"(SELECT Use FROM Use_Code WHERE Use_Code.Description = '{use}'), " +
                            $"'{detail}'); ";
                        }
                        else
                        {
                            sql = "INSERT INTO Balance_Info " +
                            " (InOut, Money, UseDate, Bank, UseWhere, Detail ) " +
                            "VALUES " +
                            $"('{inOut}', " +
                            $"{money}, " +
                            $"'{useDate}', " +
                            $"(SELECT Bank FROM Bank_Code WHERE Bank_Code.Description = '{bank}' AND Bank_Code.Kind = '{kind}'), " +
                            $"(SELECT Use FROM Use_Code WHERE Use_Code.Description = '{use}'), " +
                            $"'{detail}'); ";
                        }
                    }
                    
                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    returnData = command.ExecuteNonQuery() > 0;
                }
                if (returnData) return true;
                else return false;
            }
            catch (Exception ex)
            {
                log.Error($"{MethodBase.GetCurrentMethod()?.Name}::{ex.Message}");
                return false;
            }
        }
        public bool InsertBankCardCode(string kind, string bank, string card)
        {
            bool returnData = false;
            bool returnData2 = false;
            string path = String.Format("Data Source = {0}", filePath);

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();
                    string sql = "";

                    if (card == null)
                    {
                        sql = "INSERT INTO Bank_Code " +
                            "(Description, Kind) " +
                            "VALUES " +
                            $"('{bank}', '{kind}');";

                        SQLiteCommand command = new SQLiteCommand(sql, connection);
                        returnData = command.ExecuteNonQuery() > 0;
                        returnData2 = true;
                    }
                    else
                    {
                        sql = "INSERT INTO Bank_Code " +
                            "(Description, Kind) " +
                            "VALUES " +
                            $"('{bank}', '{kind}');";

                        SQLiteCommand command = new SQLiteCommand(sql, connection);
                        returnData = command.ExecuteNonQuery() > 0;

                        int bankNumber = 0;

                        if (returnData)
                        {
                            sql = "SELECT Bank " +
                                "FROM Bank_Code " +
                                $"WHERE Description = '{bank}' " +
                                $"AND Kind = '{kind}';";
                            command = new SQLiteCommand(sql, connection);
                            
                            SQLiteDataReader reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                log.Info(reader["Bank"].ToString());
                                bankNumber = (int)(long)reader["Bank"];
                            }
                            reader.Close();

                        }
                        string sql2 = "INSERT INTO Card_Code " +
                            "(Description, Bank) " +
                            "VALUES " +
                            $"('{card}', {bankNumber});";

                        
                        SQLiteCommand command2 = new SQLiteCommand(sql2, connection);
                        returnData2 = command2.ExecuteNonQuery() > 0;
                    }
                }
                if (returnData && returnData2) return true;
                else return false;
            }
            catch (Exception ex)
            {
                log.Error($"{MethodBase.GetCurrentMethod()?.Name}::{ex.Message}");
                return false;
            }
        }
        public bool InsertUseCode(string use)
        {
            bool returnData = false;
            string path = String.Format("Data Source = {0}", filePath);

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();
                    string sql = "INSERT INTO Use_Code " +
                            "(Description) " +
                            "VALUES " +
                            $"('{use}');";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    returnData = command.ExecuteNonQuery() > 0;
                }
                if (returnData) return true;
                else return false;
            }
            catch (Exception ex)
            {
                log.Error($"{MethodBase.GetCurrentMethod()?.Name}::{ex.Message}");
                return false;
            }
        }
        public bool InsertBankCardUseSet(string kind, string bank, string card, string use)
        {
            bool returnData = false;
            string path = String.Format("Data Source = {0}", filePath);

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();
                    string sql = "INSERT INTO Bank_Card_Use_Set " +
                            "(Bank, Card, Use, Kind) " +
                            "VALUES " +
                            $"((SELECT Bank FROM Bank_Code WHERE Description = '{bank}')," +
                            $"(SELECT Card FROM Card_Code WHERE Description = '{card}')," +
                            $"(SELECT Use FROM Use_Code WHERE Description = '{use}'), " +
                            $"'{kind}');";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    returnData = command.ExecuteNonQuery() > 0;
                }
                if (returnData) return true;
                else return false;
            }
            catch (Exception ex)
            {
                log.Error($"{MethodBase.GetCurrentMethod()?.Name}::{ex.Message}");
                return false;
            }
        }

        public List<InOutModel> SelectGroupByInOutData(string fromDate, string toDate)
        {
            List<InOutModel> returnData = new List<InOutModel>();

            string path = String.Format("Data Source = {0}", filePath);
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(path))
                {
                    connection.Open();
                    string sql = "SELECT SUM(Money) AS MONEY, A.Description AS Bank, B.Description AS Card, C.Description AS UseWhere, strftime(\"%m\", UseDate) AS UseDate " +
                        "FROM Balance_Info " +
                        "LEFT JOIN Bank_Code A ON Balance_Info.Bank = A.Bank " +
                        "LEFT JOIN Card_Code B ON Balance_Info.Card = B.Card " +
                        "LEFT JOIN Use_Code C ON Balance_Info.UseWhere = C.Use " +
                        $"WHERE Balance_Info.InOut = 'OUT' " +
                        "AND Balance_Info.UseWhere != 19 " +
                        $"AND UseDate BETWEEN '{fromDate}' AND '{toDate}'" +
                        "GROUP BY A.Description, B.Description, C.Description, strftime(\"%m\", UseDate) " +
                        "ORDER BY Bank, strftime(\"%m\", UseDate), UseWhere";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        returnData.Add(new InOutModel
                        {
                            Bank = reader["Bank"].ToString(),
                            Card = reader["Card"].ToString(),
                            Use = reader["UseWhere"].ToString(),
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
    }
}

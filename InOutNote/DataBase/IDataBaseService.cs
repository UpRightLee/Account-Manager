using InOutNote.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InOutNote.DataBase
{
    public interface IDataBaseService
    {
        public void CreateDB();
        public List<InOutModel> SelectWeeklyInOutData();
        public List<InOutModel> SelectMonthlyInOutData();
        public List<InOutModel> SelectAllInOutData(string fromDate, string ToDate, InOutModel inOutModel);
        public List<Bank> SelectBankCode();
        public List<Card> SelectCardCode();
        public List<Use> SelectUseCode();
        public List<Bank> SelectBankCardCode(Bank bank);
        public List<SummaryData> SelectBalanceInfo(string year);
        public string SelectINBalanceInfo();
        public string SelectOUTBalanceInfo();

        public bool DeleteInOutData(InOutModel inOutData);
        public bool DeleteBankCode(Bank bank);
        public bool DeleteUseCode(Use use);

        public bool InsertInOutData(InOutModel inOutData);
        public bool InsertBankCardCode(string kind, string bank, string card);
        public bool InsertUseCode(string use);
    }
}

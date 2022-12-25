﻿using InOutNote.Models;
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
        public List<String> SelectBankCode();
        public List<String> SelectCardCode();
        public List<String> SelectUseCode();
    }
}
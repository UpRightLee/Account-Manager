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
        public List<InOutModel> GetWeeklyInOutData();
        public List<InOutModel> GetMonthlyInOutData();
    }
}

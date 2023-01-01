using InOutNote.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InOutNote.ExcelManage
{
    public interface IExcelService
    {
        bool SaveInOutDataList(ObservableCollection<InOutModel> inOutDataList);
        bool SaveBankCardCodeList(ObservableCollection<Bank> bankList);
        bool SaveUseCodeList(ObservableCollection<Use> UseList);
    }
}

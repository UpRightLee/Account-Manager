using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InOutNote.WindowManage
{
    public interface IWindowService
    {
        void ShowAddInoutView();
        void CloseAddInoutView();
        void ShowAddBankCardCodeView();
        void CloseAddBankCardCodeView();
        void ShowAddUseCodeView();
        void CloseAddUseCodeView();
        void ShowAddSetView();
        void CloseAddSetView();
    }
}

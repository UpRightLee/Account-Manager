using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InOutNote.WindowManage
{
    public class MessageBoxService : IMessageBoxService
    {
        private static MessageBoxService instance = new MessageBoxService();
        public static MessageBoxService Instance
        {
            get
            {
                if (instance == null) instance= new MessageBoxService();
                return instance;
            }
        }
        public void ShowMessageBox(string message)
        {
            MessageBox.Show(message);
        }
        public bool ShowYesOrNoMessageBox(string message)
        {
            if (MessageBox.Show(message, "YesOrNo", MessageBoxButton.OKCancel) == MessageBoxResult.OK) return true;
            else return false;
        }
    }
}

using InOutNote.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InOutNote.WindowManage
{
    public class WindowService : IWindowService
    {
        private static WindowService instance = new WindowService();

        private AddInOutView? addInOutView = null;
        private AddBankCardCodeView? addBankCardCodeView = null;
        private AddUseCodeView? addUseCodeView = null;
        private AddSetView? addSetView = null;

        public static WindowService Instance
        {
            get 
            { 
                if (instance == null) instance= new WindowService();
                return instance;
            }
            
        }    
        public void ShowAddInoutView()
        {
            if (addInOutView == null)
            {
                addInOutView = new AddInOutView();
                addInOutView.ShowDialog();
            }
            else addInOutView.ShowDialog();
        }
        public void CloseAddInoutView()
        {
            if (addInOutView != null)
            {
                addInOutView.Close();
                addInOutView = null;
            }
        }

        public void ShowAddBankCardCodeView()
        {
            if (addBankCardCodeView == null)
            {
                addBankCardCodeView = new AddBankCardCodeView();
                addBankCardCodeView.ShowDialog();
            }
            else addBankCardCodeView.ShowDialog();
        }
        public void CloseAddBankCardCodeView()
        {
            if (addBankCardCodeView != null)
            {
                addBankCardCodeView.Close();
                addBankCardCodeView = null;
            }
        }

        public void ShowAddUseCodeView()
        {
            if (addUseCodeView == null)
            {
                addUseCodeView = new AddUseCodeView();
                addUseCodeView.ShowDialog();
            }
            else addUseCodeView.ShowDialog();
        }

        public void CloseAddUseCodeView()
        {
            if (addUseCodeView != null)
            {
                addUseCodeView.Close();
                addUseCodeView = null;
            }
        }

        public void ShowAddSetView()
        {
            if (addSetView == null)
            {
                addSetView = new AddSetView();
                addSetView.ShowDialog();
            }
            else addSetView.ShowDialog();
        }

        public void CloseAddSetView()
        {
            if (addSetView != null)
            {
                addSetView.Close();
                addSetView = null;
            }
        }
    }
}

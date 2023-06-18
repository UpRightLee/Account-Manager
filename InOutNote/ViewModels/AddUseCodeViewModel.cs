using InOutNote.Command;
using InOutNote.DataBase;
using InOutNote.Notifier;
using InOutNote.WindowManage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InOutNote.ViewModels
{
    public class AddUseCodeViewModel : INotifier
    {
        private static IDataBaseService dataBaseService = DataBaseService.Instance;
        private static IWindowService windowService = WindowService.Instance;
        private static IMessageBoxService messageBoxService = MessageBoxService.Instance;

        private string selectedUse = "";

       
        public string SelectedUse
        {
            get { return selectedUse; }
            set
            {
                selectedUse = value;
                OnPropertyChanged("SelectedUse");
            }
        }
        
        public RelayCommand LoadAddCodeViewCommand { get; }
        public RelayCommand AddDataCommand { get; }
        public RelayCommand CancelDataCommand { get; }
        public RelayCommand UnloadAddCodeViewCommand { get; }
        public RelayCommand CloseByESCCommand { get; }
        public AddUseCodeViewModel()
        {
            LoadAddCodeViewCommand = new RelayCommand(LoadAddCodeView);
            AddDataCommand = new RelayCommand(AddData);
            CancelDataCommand = new RelayCommand(CancelData);
            UnloadAddCodeViewCommand = new RelayCommand(UnloadAddCodeView);
            CloseByESCCommand = new RelayCommand(CloseByESC);
        }

        private void CloseByESC()
        {
            windowService.CloseAddUseCodeView();
        }

        private void LoadAddCodeView()
        {
        }

        private void UnloadAddCodeView()
        {
            windowService.CloseAddUseCodeView();
        }

        private void AddData()
        {
            if (dataBaseService.InsertUseCode(SelectedUse)) messageBoxService.ShowMessageBox("Insert Use Code Success");
            else messageBoxService.ShowMessageBox("Insert Failed");
        }

        private void CancelData()
        {
            windowService.CloseAddUseCodeView();
        }
    }
}

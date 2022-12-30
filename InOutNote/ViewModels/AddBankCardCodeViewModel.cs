using InOutNote.Command;
using InOutNote.DataBase;
using InOutNote.Models;
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
    public class AddBankCardCodeViewModel : INotifier
    {
        private static IDataBaseService dataBaseService = DataBaseService.Instance;
        private static IWindowService windowService = WindowService.Instance;
        private static IMessageBoxService messageBoxService = MessageBoxService.Instance;

        private ObservableCollection<string> kind = new ObservableCollection<string>();

        private string selectedKind = "";
        private string selectedBank = "";
        private string selectedCard = "";

        public ObservableCollection<string> Kind
        {
            get { return kind; }
            set
            {
                kind = value;
                OnPropertyChanged("Kind");
            }
        }

        public string SelectedKind
        {
            get { return selectedKind; }
            set
            {
                selectedKind = value;
                OnPropertyChanged("SelectedKind");
            }
        }
        public string SelectedCard
        {
            get { return selectedCard; }
            set
            {
                selectedCard = value;
                OnPropertyChanged("SelectedCard");
            }
        }
        public string SelectedBank
        {
            get { return selectedBank; }
            set
            {
                selectedBank = value;
                OnPropertyChanged("SelectedBank");
            }
        }

        public RelayCommand LoadAddBankCardViewCommand { get; }
        public RelayCommand AddDataCommand { get; }
        public RelayCommand CancelDataCommand { get; }
        public RelayCommand UnloadAddBankCardViewCommand { get; }

        public AddBankCardCodeViewModel()
        {
            LoadAddBankCardViewCommand = new RelayCommand(LoadAddBankCardView);
            AddDataCommand = new RelayCommand(AddData);
            CancelDataCommand = new RelayCommand(CancelData);
            UnloadAddBankCardViewCommand = new RelayCommand(UnloadAddBankCardView);
        }

        private void LoadAddBankCardView()
        {
            Kind = new ObservableCollection<string>();
            Kind.Add("신용");
            Kind.Add("체크");
            Kind.Add("자동이체");
            SelectedKind = "신용";
        }

        private void UnloadAddBankCardView()
        {
            Kind.Clear();
            windowService.CloseAddBankCardCodeView();
        }

        private void AddData()
        {
            if (dataBaseService.InsertBankCardCode(SelectedKind, SelectedBank, SelectedCard)) messageBoxService.ShowMessageBox("Insert Bank, Card Code Success");
            else messageBoxService.ShowMessageBox("Insert Failed");
        }

        private void CancelData()
        {
            windowService.CloseAddBankCardCodeView();
        }
    }
}

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
    public class AddSetViewModel : INotifier
    {
        private static IDataBaseService dataBaseService = DataBaseService.Instance;
        private static IWindowService windowService = WindowService.Instance;
        private static IMessageBoxService messageBoxService = MessageBoxService.Instance;

        private ObservableCollection<string> useList = new ObservableCollection<string>();
        private ObservableCollection<string> bankList = new ObservableCollection<string>();
        private ObservableCollection<string> cardList = new ObservableCollection<string>();
        private ObservableCollection<string> kindList = new ObservableCollection<string>();

        private string selectedUse = "";
        private string selectedBank = "";
        private string selectedCard = "";
        private string selectedKind = "";

        public ObservableCollection<string> UseList
        {
            get { return useList; }
            set
            {
                useList = value;
                OnPropertyChanged("UseList");
            }
        }
        public ObservableCollection<string> BankList
        {
            get { return bankList; }
            set
            {
                bankList = value;
                OnPropertyChanged("BankList");
            }
        }
        public ObservableCollection<string> CardList
        {
            get { return cardList; }
            set
            {
                cardList = value;
                OnPropertyChanged("CardList");
            }
        }
        public ObservableCollection<string> KindList
        {
            get { return kindList; }
            set
            {
                kindList = value;
                OnPropertyChanged("KindList");
            }
        }
        public string SelectedUse
        {
            get { return selectedUse; }
            set
            {
                selectedUse = value;
                OnPropertyChanged("SelectedUse");
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
                IsChangedBank();
            }
        }
        public string SelectedKind
        {
            get { return selectedKind; }
            set
            {
                selectedKind = value;
                OnPropertyChanged("SelectedKind");
                IsChangedKind();
            }
        }

        public RelayCommand LoadAddSetViewCommand { get; }
        public RelayCommand AddDataCommand { get; }
        public RelayCommand CancelDataCommand { get; }
        public RelayCommand UnloadAddSetViewCommand { get; }
        public RelayCommand CloseByESCCommand { get; }
        public AddSetViewModel()
        {
            LoadAddSetViewCommand = new RelayCommand(LoadAddSetView);
            AddDataCommand = new RelayCommand(AddData);
            CancelDataCommand = new RelayCommand(CancelData);
            UnloadAddSetViewCommand = new RelayCommand(UnloadAddSetView);
            CloseByESCCommand = new RelayCommand(CloseByESC);
        }

        private void CloseByESC()
        {
            UnloadAddSetView();
        }

        private void LoadAddSetView()
        {
            KindList = new ObservableCollection<string>();
            KindList.Add("신용");
            KindList.Add("체크");
            KindList.Add("자동이체");
            SelectedKind = "신용";

            UseList = new ObservableCollection<string>();

            List<Use> returnUse = dataBaseService.SelectUseCode();
            for (int i = 0; i < returnUse.Count; i++)
            {
                UseList.Add(returnUse[i].Description!);
            }
            SelectedUse = UseList[0];
        }

        private void UnloadAddSetView()
        {
            BankList.Clear();
            CardList.Clear();
            UseList.Clear();

            windowService.CloseAddSetView();
        }

        private void AddData()
        {
            if (dataBaseService.InsertBankCardUseSet(SelectedKind, SelectedBank, SelectedCard, SelectedUse)) messageBoxService.ShowMessageBox("Insert 즐겨찾기 Success");
            else messageBoxService.ShowMessageBox("Insert Failed");
        }

        private void CancelData()
        {
            UnloadAddSetView();
        }
        private void IsChangedKind()
        {
            List<Bank> returnBank = dataBaseService.SelectBankCode();
            BankList = new ObservableCollection<string>();
            List<Card> returnCard = dataBaseService.SelectCardCode();
            CardList = new ObservableCollection<string>();

            for (int i = 0; i < returnBank.Count; i++)
            {
                if (returnBank[i].Kind == SelectedKind)
                {
                    if (!BankList.Contains(returnBank[i].Description!)) BankList.Add(returnBank[i].Description!);
                    for (int k = 0; k < returnCard.Count; k++)
                    {
                        if ((returnBank[i].Name == returnCard[k].Bank) && !CardList.Contains(returnCard[k].Description!)) CardList.Add(returnCard[k].Description!);
                    }
                }
            }
            if (BankList.Count > 0) SelectedBank = BankList[0];
            if (CardList.Count > 0)
            {
                SelectedCard = CardList[0];
            }
        }
        private void IsChangedBank()
        {
            Bank selectedBankCard = new Bank
            {
                Description = SelectedBank,
                Kind = SelectedKind
            };

            List<Bank> returnBank = dataBaseService.SelectBankCardCode(selectedBankCard);
            CardList = new ObservableCollection<string>();

            for (int i = 0; i < returnBank.Count; i++)
            {
                if (returnBank[i].Card != "") CardList.Add(returnBank[i].Card!);
            }
            if (CardList.Count > 0)
            {
                SelectedCard = CardList[0];
            }
        }
    }
}

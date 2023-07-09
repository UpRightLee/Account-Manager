using InOutNote.Command;
using InOutNote.DataBase;
using InOutNote.Models;
using InOutNote.Notifier;
using InOutNote.WindowManage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InOutNote.ViewModels
{
    public class AddInOutViewModel : INotifier
    {
        private DateTime selectedDate;

        private static IDataBaseService dataBaseService = DataBaseService.Instance;
        private static IWindowService windowService = WindowService.Instance;
        private static IMessageBoxService messageBoxService = MessageBoxService.Instance;

        private ObservableCollection<string> inOut = new ObservableCollection<string>();
        private ObservableCollection<string> kind = new ObservableCollection<string>();
        private ObservableCollection<string> bank = new ObservableCollection<string>();
        private ObservableCollection<string> card = new ObservableCollection<string>();
        private ObservableCollection<string> use = new ObservableCollection<string>();

        private List<BankCardUseSet> bankCardUseSets = new List<BankCardUseSet>();

        private string selectedInOut = "";
        private string selectedKind = "";
        private string selectedBank = "";
        private string selectedCard = "";
        private string selectedUse = "";
        private string selectedMoney = "";
        private string selectedDetail = "";
        private bool isCardEnabled = false;

        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set
            {
                selectedDate = value;
                OnPropertyChanged("SelectedDate");
            }
        }
        public bool IsCardEnabled
        {
            get { return isCardEnabled; }
            set
            {
                isCardEnabled = value;
                OnPropertyChanged("IsCardEnabled");
            }
        }
        public ObservableCollection<string> InOut
        {
            get { return inOut; }
            set
            {
                inOut = value;
                OnPropertyChanged("InOut");
            }
        }
        public ObservableCollection<string> Kind
        {
            get { return kind; }
            set
            {
                kind = value;
                OnPropertyChanged("Kind");
            }
        }
        public ObservableCollection<string> Card
        {
            get { return card; }
            set
            {
                card = value;
                OnPropertyChanged("Card");
            }
        }
        public ObservableCollection<string> Bank
        {
            get { return bank; }
            set
            {
                bank = value;
                OnPropertyChanged("Bank");
            }
        }
        public ObservableCollection<string> Use
        {
            get { return use; }
            set
            {
                use = value;
                OnPropertyChanged("Use");
            }
        }
        public string SelectedInOut
        {
            get { return selectedInOut; }
            set
            {
                selectedInOut = value;
                OnPropertyChanged("SelectedInOut");
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
        public string SelectedUse
        {
            get { return selectedUse; }
            set
            {
                selectedUse = value;
                OnPropertyChanged("SelectedUse");
            }
        }
        public string SelectedMoney
        {
            get { return selectedMoney; }
            set
            {
                selectedMoney = value;
                OnPropertyChanged("SelectedMoney");
            }
        }
        public string SelectedDetail
        {
            get { return selectedDetail; }
            set
            {
                selectedDetail = value;
                OnPropertyChanged("SelectedDetail");
            }
        }
        public RelayCommand LoadAddInOutViewCommand { get; }
        public RelayCommand AddDataCommand { get; }
        public RelayCommand CancelDataCommand { get; }
        public RelayCommand UnloadAddInOutViewCommand { get; }
        public RelayCommand CloseByESCCommand { get; }

        public AddInOutViewModel()
        {
            LoadAddInOutViewCommand = new RelayCommand(LoadInOutView);
            AddDataCommand = new RelayCommand(AddData);
            CancelDataCommand = new RelayCommand(CancelData);
            UnloadAddInOutViewCommand = new RelayCommand(UnloadInOutView);
            CloseByESCCommand = new RelayCommand(CloseByESC);
        }

        private void CloseByESC()
        {
            windowService.CloseAddInoutView();
        }

        private void LoadInOutView()
        {
            SelectedDate = DateTime.Now;

            InOut = new ObservableCollection<string>();
            InOut.Add("입금");
            InOut.Add("출금");
            SelectedInOut = "출금";

            Kind = new ObservableCollection<string>();
            Kind.Add("신용");
            Kind.Add("체크");
            Kind.Add("자동이체");
            SelectedKind = "신용";

            List<Use> returnUse = dataBaseService.SelectUseCode();
            Use = new ObservableCollection<string>();
            for (int i = 0; i < returnUse.Count; i++)
            {
                Use.Add(returnUse[i].Description!);
            }  
        }

        private void UnloadInOutView()
        {
            InOut.Clear();
            Use.Clear();
            Bank.Clear();
            Card.Clear();

            windowService.CloseAddInoutView();
        }

        private void AddData()
        {
            InOutModel inOutData = new InOutModel
            {
                InOut = SelectedInOut,
                Bank = SelectedBank,
                Card = SelectedCard,
                UseDate = SelectedDate.ToString("yyyy-MM-dd"),
                Money = SelectedMoney,
                Use = SelectedUse,
                Detail = SelectedDetail,
                Kind = SelectedKind
            };
            if (dataBaseService.InsertInOutData(inOutData)) messageBoxService.ShowMessageBox("Insert Data Success");
            else messageBoxService.ShowMessageBox("Insert Failed");
        }
        private void CancelData()
        {
            windowService.CloseAddInoutView();
        }
        private void IsChangedKind()
        {
            List<Bank> returnBank = dataBaseService.SelectBankCode();
            Bank = new ObservableCollection<string>();
            List<Card> returnCard = dataBaseService.SelectCardCode();
            Card = new ObservableCollection<string>();

            for (int i = 0; i < returnBank.Count; i++)
            {
                if (returnBank[i].Kind == SelectedKind)
                {
                    if (!Bank.Contains(returnBank[i].Description!)) Bank.Add(returnBank[i].Description!);
                    for (int k = 0; k < returnCard.Count; k++)
                    {
                        if ((returnBank[i].Name == returnCard[k].Bank) && !Card.Contains(returnCard[k].Description!)) Card.Add(returnCard[k].Description!);
                    }
                }
            }
            if (Bank.Count > 0) SelectedBank = Bank[0];
            if (Card.Count > 0)
            {
                IsCardEnabled = true;
                SelectedCard = Card[0];
            }
            else IsCardEnabled = false;

            bankCardUseSets = dataBaseService.SelectBankCardUseSetList();

            for (int i = 0; i < bankCardUseSets.Count; i++)
            {
                if (SelectedCard != null)
                {
                    if (bankCardUseSets[i].KindName == SelectedKind &&
                        bankCardUseSets[i].BankName == SelectedBank &&
                        bankCardUseSets[i].CardName == SelectedCard)
                    {
                        SelectedUse = bankCardUseSets[i].UseName!;
                    }
                }
                else
                {
                    if (bankCardUseSets[i].KindName == SelectedKind &&
                        bankCardUseSets[i].BankName == SelectedBank)
                    {
                        SelectedUse = bankCardUseSets[i].UseName!;
                    }
                }
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
            Card = new ObservableCollection<string>();

            for (int i = 0; i < returnBank.Count; i++)
            {
                if (returnBank[i].Card != "") Card.Add(returnBank[i].Card!);
            }
            if (Card.Count > 0)
            {
                IsCardEnabled = true;
                SelectedCard = Card[0];
            }
            else IsCardEnabled = false;

            bankCardUseSets = dataBaseService.SelectBankCardUseSetList();

            for (int i = 0; i < bankCardUseSets.Count; i++)
            {
                if (SelectedCard != null)
                {
                    if (bankCardUseSets[i].KindName == SelectedKind &&
                        bankCardUseSets[i].BankName == SelectedBank &&
                        bankCardUseSets[i].CardName == SelectedCard)
                    {
                        SelectedUse = bankCardUseSets[i].UseName!;
                    }
                }
                else
                {
                    if (bankCardUseSets[i].KindName == SelectedKind &&
                        bankCardUseSets[i].BankName == SelectedBank)
                    {
                        SelectedUse = bankCardUseSets[i].UseName!;
                    }
                }
            }
        }
    }
}

using InOutNote.Command;
using InOutNote.DataBase;
using InOutNote.Models;
using InOutNote.Notifier;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InOutNote.ViewModels
{
    public class AddInOutViewModel : INotifier
    {
        private DateTime selectedDate;

        private static IDataBaseService dataBaseService = DataBaseService.Instance;

        private ObservableCollection<string> inOut = new ObservableCollection<string>();
        private ObservableCollection<string> kind = new ObservableCollection<string>();
        private ObservableCollection<string> bank = new ObservableCollection<string>();
        private ObservableCollection<string> card = new ObservableCollection<string>();
        private ObservableCollection<string> use = new ObservableCollection<string>();

        private string selectedInOut = "";
        private string selectedKind = "";
        private string selectedBank = "";
        private string selectedCard = "";
        private string selectedUse = "";
        private string selectedMoney = "";
        private string selectedDetail = "";
        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set
            {
                selectedDate = value;
                OnPropertyChanged("SelectedDate");
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

        public AddInOutViewModel()
        {
            LoadAddInOutViewCommand = new RelayCommand(LoadInOutView);
            AddDataCommand = new RelayCommand(AddData);
            CancelDataCommand = new RelayCommand(CancelData);
            UnloadAddInOutViewCommand = new RelayCommand(UnloadInOutView);
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
            SelectedUse = Use[0];
        }

        private void UnloadInOutView()
        {
            InOut.Clear();
            Use.Clear();
            Bank.Clear();
            Card.Clear();
        }

        private void AddData()
        {
            throw new NotImplementedException();
        }

        private void CancelData()
        {
            throw new NotImplementedException();
        }
        public void IsChangedKind()
        {
            List<Bank> returnBank = dataBaseService.SelectBankCode();
            Bank = new ObservableCollection<string>();
            List<Card> returnCard = dataBaseService.SelectCardCode();
            Card = new ObservableCollection<string>();

            for (int i = 0; i < returnBank.Count; i++)
            {
                if (returnBank[i].Kind == SelectedKind)
                {
                    Bank.Add(returnBank[i].Description!);
                    for (int k = 0; k < returnCard.Count; k++)
                    {
                        if (returnBank[i].Name == returnCard[k].Bank) Card.Add(returnCard[k].Description!);
                    }
                }
            }
            if (Bank.Count > 0) SelectedBank = Bank[0];
            if (Card.Count > 0) SelectedCard = Card[0];
        }
    }
}

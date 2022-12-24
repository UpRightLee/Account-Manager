using InOutNote.Command;
using InOutNote.Notifier;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InOutNote.ViewModels
{
    public class InOutViewModel : INotifier
    {
        private DateTime selectedFromDate;
        private DateTime selectedToDate;
        
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

        public DateTime SelectedFromDate
        {
            get { return selectedFromDate; }
            set 
            {
                selectedFromDate = value;
                OnPropertyChanged("SelectedFromDate");
            }
        }
        public DateTime SelectedToDate
        {
            get { return selectedToDate; }
            set
            {
                selectedToDate = value;
                OnPropertyChanged("SelectedToDate");
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
        public RelayCommand LoadInOutViewCommand { get; }
        public RelayCommand SelectDataCommand { get; }
        public RelayCommand ExcelDownloadCommand { get; }
        public RelayCommand AddDataCommand { get; }
        public RelayCommand DeleteDataCommand { get; }

        public InOutViewModel()
        {
            LoadInOutViewCommand = new RelayCommand(LoadInOutView);
            SelectDataCommand = new RelayCommand(SelectData);
            ExcelDownloadCommand = new RelayCommand(ExcelDownload);
            AddDataCommand = new RelayCommand(AddData);
            DeleteDataCommand = new RelayCommand(DeleteData);
        }

        private void DeleteData()
        {
            Console.WriteLine("Delete Data Command");
        }

        private void AddData()
        {
            Console.WriteLine("Add Data Command");
        }

        private void ExcelDownload()
        {
            Console.WriteLine("ExcelDownload Command");
        }

        private void SelectData()
        {
            Console.WriteLine("Select Data Command");
        }

        private void LoadInOutView()
        {
            SelectedToDate = DateTime.Now;
            SelectedFromDate = DateTime.Now.AddDays(-7);

            InOut = new ObservableCollection<string>();
            InOut.Add("입금");
            InOut.Add("출금");
            InOut.Add("전체");
            SelectedInOut = "전체";

            Kind = new ObservableCollection<string>();
            Kind.Add("신용");
            Kind.Add("체크");
            Kind.Add("자동이체");
            Kind.Add("전체");
            SelectedKind = "전체";

            Card = new ObservableCollection<string>();
            Card.Add("IBK신용카드");
            Card.Add("국민체크카드");
            Card.Add("전체");
            SelectedCard = "전체";

            Bank = new ObservableCollection<string>();
            Bank.Add("국민은행");
            Bank.Add("기업은행");
            Bank.Add("전체");
            SelectedBank = "전체";

            Use = new ObservableCollection<string>();
            Use.Add("생활비");
            Use.Add("가스비");
            Use.Add("전체");
            SelectedUse = "전체";
        }
    }
}

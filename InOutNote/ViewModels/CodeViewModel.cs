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
    public class CodeViewModel : INotifier
    {
        private static IDataBaseService dataBaseService = DataBaseService.Instance;

        private ObservableCollection<Bank> bankList = new ObservableCollection<Bank>();
        private ObservableCollection<Use> useList = new ObservableCollection<Use>();

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

        public ObservableCollection<Bank> BankList
        {
            get { return bankList;}
            set
            {
                bankList = value;
                OnPropertyChanged("BankList");
            }
        }
        public ObservableCollection<Use> UseList
        {
            get { return useList; }
            set
            {
                useList = value;
                OnPropertyChanged("UseList");
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
        public RelayCommand LoadCodeViewCommand { get; }
        public RelayCommand SelectDataCommand { get; }
        public RelayCommand ExcelDownloadCommand { get; }
        public RelayCommand AddDataCommand { get; }
        public RelayCommand DeleteDataCommand { get; }
        public CodeViewModel()
        {
            LoadCodeViewCommand = new RelayCommand(LoadCodeView);
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

        private void LoadCodeView()
        {
            Kind = new ObservableCollection<string>();
            Kind.Add("신용");
            Kind.Add("체크");
            Kind.Add("자동이체");
            Kind.Add("전체");
            SelectedKind = "전체";

            List<Card> returnCard = dataBaseService.SelectCardCode();
            Card = new ObservableCollection<string>();
            for (int i = 0; i < returnCard.Count; i++)
            {
                Card.Add(returnCard[i].Description!);
            }
            Card.Add("전체");
            SelectedCard = "전체";


            List<Bank> returnBank = dataBaseService.SelectBankCode();
            Bank = new ObservableCollection<string>();
            BankList = new ObservableCollection<Bank>();
            for (int i = 0; i < returnBank.Count; i++)
            {
                Bank.Add(returnBank[i].Description!);
                BankList.Add( new Bank
                    {
                        Kind = returnBank[i].Kind,
                        Description = returnBank[i].Description,
                        Card = returnBank[i].Card
                    });
            }
            Bank.Add("전체");
            SelectedBank = "전체";

            List<Use> returnUse = dataBaseService.SelectUseCode();
            Use = new ObservableCollection<string>();
            UseList = new ObservableCollection<Use>();
            for (int i = 0; i < returnUse.Count; i++)
            {
                Use.Add(returnUse[i].Description!);
                UseList.Add(new Use
                {
                    Name= returnUse[i].Name,
                    Description= returnUse[i].Description
                });
            }
            Use.Add("전체");
            SelectedUse = "전체";
        }
    }
}

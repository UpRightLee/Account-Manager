using InOutNote.Command;
using InOutNote.DataBase;
using InOutNote.Models;
using InOutNote.Notifier;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        private ObservableCollection<string> kind = new ObservableCollection<string>();
        private ObservableCollection<string> bank = new ObservableCollection<string>();

        private string selectedKind = "";
        private string selectedBank = "";

        private Use? selectedRowKind;
        private Bank? selectedRowBank;

        public Bank SelectedRowBank
        {
            get { return selectedRowBank!; }
            set
            {
                selectedRowBank = value;
                OnPropertyChanged("SelectedRowBank");
            }
        }
        public Use SelectedRowUse
        {
            get { return selectedRowKind!; }
            set
            {
                selectedRowKind = value;
                OnPropertyChanged("RowKind");
            }
        }

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
        public ObservableCollection<string> Kind
        {
            get { return kind; }
            set
            {
                kind = value;
                OnPropertyChanged("Kind");
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
        public string SelectedKind
        {
            get { return selectedKind; }
            set
            {
                selectedKind = value;
                OnPropertyChanged("SelectedKind");
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
        public RelayCommand LoadCodeViewCommand { get; }
        public RelayCommand SelectDataCommand { get; }
        public RelayCommand ExcelDownloadCommand { get; }
        public RelayCommand AddDataCommand { get; }
        public RelayCommand DeleteDataCommand { get; }
        public RelayCommand UnloadCodeViewCommand { get; }
        public CodeViewModel()
        {
            LoadCodeViewCommand = new RelayCommand(LoadCodeView);
            SelectDataCommand = new RelayCommand(SelectData);
            ExcelDownloadCommand = new RelayCommand(ExcelDownload);
            AddDataCommand = new RelayCommand(AddData);
            DeleteDataCommand = new RelayCommand(DeleteData);
            UnloadCodeViewCommand = new RelayCommand(UnloadCodeView);
        }
 
        private void DeleteData()
        {
            if (SelectedRowBank != null)
            {
                if (dataBaseService.DeleteBankCode(SelectedRowBank)) Debug.WriteLine("========== Delete Bank AND Card Success ==========");
                else Debug.WriteLine("========== Delete Bank AND Card Fail ==========");
            }
            if (SelectedRowUse != null)
            {
                if (dataBaseService.DeleteUseCode(SelectedRowUse)) Debug.WriteLine("========== Delete Use Success ==========");
                else Debug.WriteLine("========== Delete Use Fail ==========");
            }
            SelectData();
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
            Bank selectedBankCard = new Bank
            {
                Description = SelectedBank,
                Kind = SelectedKind
            };

            List<Bank> returnBank = dataBaseService.SelectBankCardCode(selectedBankCard);  
            BankList = new ObservableCollection<Bank>();
            for (int i = 0; i < returnBank.Count; i++)
            {
                BankList.Add(new Bank
                {
                    Kind = returnBank[i].Kind,
                    Description = returnBank[i].Description,
                    Card = returnBank[i].Card
                });
            }

            List<Use> returnUse = dataBaseService.SelectUseCode();
            UseList = new ObservableCollection<Use>();
            for (int i = 0; i < returnUse.Count; i++)
            {
                UseList.Add(new Use
                {
                    Name = returnUse[i].Name,
                    Description = returnUse[i].Description
                });
            }
        }

        private void LoadCodeView()
        {
            Kind = new ObservableCollection<string>();
            Kind.Add("신용");
            Kind.Add("체크");
            Kind.Add("자동이체");
            Kind.Add("전체");
            SelectedKind = "전체";

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
            UseList = new ObservableCollection<Use>();
            for (int i = 0; i < returnUse.Count; i++)
            {
                UseList.Add(new Use
                {
                    Name= returnUse[i].Name,
                    Description= returnUse[i].Description
                });
            }
        }
        private void UnloadCodeView()
        {
            BankList.Clear();
            UseList.Clear();
            Bank.Clear();
            Kind.Clear();
        }
    }
}

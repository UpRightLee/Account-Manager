using InOutNote.Command;
using InOutNote.DataBase;
using InOutNote.ExcelManage;
using InOutNote.Models;
using InOutNote.Notifier;
using InOutNote.WindowManage;
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
        private static IWindowService windowService = WindowService.Instance;
        private static IMessageBoxService messageBoxService = MessageBoxService.Instance;
        private static IExcelService excelService = ExcelService.Instance;

        private ObservableCollection<Bank> bankList = new ObservableCollection<Bank>();
        private ObservableCollection<Use> useList = new ObservableCollection<Use>();

        private ObservableCollection<string> kind = new ObservableCollection<string>();
        private ObservableCollection<string> bank = new ObservableCollection<string>();

        private string selectedKind = "";
        private string selectedBank = "";
        private string selectedBankOrKind = "";

        private Use? selectedRowKind;
        private Bank? selectedRowBank;

        private ObservableCollection<string> bankOrKind = new ObservableCollection<string>();

        public ObservableCollection<string> BankOrKind
        {
            get { return bankOrKind; }
            set
            {
                bankOrKind = value;
                OnPropertyChanged("BankOrKind");
            }
        }
        public string SelectedBankOrKind
        {
            get { return selectedBankOrKind; }
            set
            {
                selectedBankOrKind = value;
                OnPropertyChanged("SelectedBankOrKind");
            }
        }
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
                IsChangedKind();
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
                if (dataBaseService.DeleteBankCode(SelectedRowBank)) messageBoxService.ShowMessageBox("========== Delete Bank AND Card Success ==========");
                else messageBoxService.ShowMessageBox("========== Delete Bank AND Card Fail ==========");
            }
            if (SelectedRowUse != null)
            {
                if (dataBaseService.DeleteUseCode(SelectedRowUse)) messageBoxService.ShowMessageBox("========== Delete Use Success ==========");
                else messageBoxService.ShowMessageBox("========== Delete Use Fail ==========");
            }
            SelectData();
        }

        private void AddData()
        {
            if (SelectedBankOrKind == "은행 및 카드") windowService.ShowAddBankCardCodeView();
            else windowService.ShowAddUseCodeView();
        }

        private void ExcelDownload()
        {
            if (SelectedBankOrKind == "은행 및 카드")
            {
                if (excelService.SaveBankCardCodeList(BankList)) messageBoxService.ShowMessageBox("========== Excel Save Success ==========");
                else messageBoxService.ShowMessageBox("========== Excel Save Fail ==========");
            }
            else
            {
                if (excelService.SaveUseCodeList(UseList)) messageBoxService.ShowMessageBox("========== Excel Save Success ==========");
                else messageBoxService.ShowMessageBox("========== Excel Save Fail ==========");
            }
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
            BankOrKind = new ObservableCollection<string>();
            BankOrKind.Add("은행 및 카드");
            BankOrKind.Add("용도");
            SelectedBankOrKind = "은행 및 카드";

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
                if (!Bank.Contains(returnBank[i].Description!)) Bank.Add(returnBank[i].Description!);
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
        private void IsChangedKind()
        {
            List<Bank> returnBank = dataBaseService.SelectBankCode();
            Bank = new ObservableCollection<string>();

            for (int i = 0; i < returnBank.Count; i++)
            {
                if (SelectedKind == "전체") if (!Bank.Contains(returnBank[i].Description!)) Bank.Add(returnBank[i].Description!);
                else if (returnBank[i].Kind == SelectedKind)
                {
                    if (!Bank.Contains(returnBank[i].Description!)) Bank.Add(returnBank[i].Description!);
                }
            }
            Bank.Add("전체");
            SelectedBank = "전체";
        }
    }
}

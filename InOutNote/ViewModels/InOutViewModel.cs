using InOutNote.Command;
using InOutNote.DataBase;
using InOutNote.Models;
using InOutNote.Notifier;
using InOutNote.Views;
using InOutNote.WindowManage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using InOutNote.ExcelManage;

namespace InOutNote.ViewModels
{
    public class InOutViewModel : INotifier
    {
        private DateTime selectedFromDate;
        private DateTime selectedToDate;

        private static IDataBaseService dataBaseService = DataBaseService.Instance;
        private static IWindowService windowService = WindowService.Instance;
        private static IMessageBoxService messageBoxService = MessageBoxService.Instance;
        private static IExcelService excelService = ExcelService.Instance;

        private ObservableCollection<InOutModel> inOutList = new ObservableCollection<InOutModel>();

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
        private string outTotalSum = "";
        private string totalSum = "";

        private InOutModel selectedInOutData= new InOutModel();

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
        public InOutModel SelectedInOutData
        {
            get { return selectedInOutData; }
            set
            {
                selectedInOutData = value;
                OnPropertyChanged("SelectedInOutData");
            }
        }
        public ObservableCollection<InOutModel> InOutList
        {
            get { return inOutList; }
            set
            {
                inOutList = value;
                OnPropertyChanged("InOutList");
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
        public string OutTotalSum
        {
            get { return outTotalSum; }
            set
            {
                outTotalSum = value;
                OnPropertyChanged("OutTotalSum");
            }
        }
        public string TotalSum
        {
            get { return totalSum; }
            set
            {
                totalSum = value;
                OnPropertyChanged("TotalSum");
            }
        }
        public RelayCommand LoadInOutViewCommand { get; }
        public RelayCommand SelectDataCommand { get; }
        public RelayCommand ExcelDownloadCommand { get; }
        public RelayCommand AddDataCommand { get; }
        public RelayCommand DeleteDataCommand { get; }
        public RelayCommand UnloadInOutViewCommand { get; }

        public InOutViewModel()
        {
            LoadInOutViewCommand = new RelayCommand(LoadInOutView);
            SelectDataCommand = new RelayCommand(SelectData);
            ExcelDownloadCommand = new RelayCommand(ExcelDownload);
            AddDataCommand = new RelayCommand(AddData);
            DeleteDataCommand = new RelayCommand(DeleteData);
            UnloadInOutViewCommand = new RelayCommand(UnloadInOutView);
        }

        private void DeleteData()
        {
            if (SelectedInOutData.InOut == null)
            {
                messageBoxService.ShowMessageBox("데이터를 선택해주세요");
                return;
            }
            if (dataBaseService.DeleteInOutData(SelectedInOutData)) messageBoxService.ShowMessageBox("========== Delete Success ==========");
            else messageBoxService.ShowMessageBox("========== Delete Fail ==========");

            SelectData();
        }

        private void AddData()
        {
            windowService.ShowAddInoutView();
        }

        private void ExcelDownload()
        {
            if (excelService.SaveInOutDataList(InOutList)) messageBoxService.ShowMessageBox("========== Excel Save Success ==========");
            else messageBoxService.ShowMessageBox("========== Excel Save Fail ==========");
        }

        private void SelectData()
        {
            List<InOutModel> returnInOutData = dataBaseService.SelectAllInOutData(
                SelectedFromDate.ToString("yyyy-MM-dd"), SelectedToDate.ToString("yyyy-MM-dd"),
                new InOutModel
                {
                    Bank = SelectedBank,
                    Card = SelectedCard,
                    Kind = SelectedKind,
                    Use = SelectedUse,
                    InOut = SelectedInOut
                });

            InOutList = new ObservableCollection<InOutModel>();

            int inTotal = 0;
            int outTotal = 0;

            for (int i = 0; i < returnInOutData.Count; i++)
            {
                InOutList.Add(
                    new InOutModel
                    {
                        InOut = returnInOutData[i].InOut,
                        Money = String.Format("{0:#,###}", int.Parse(returnInOutData[i].Money!)),
                        Kind = returnInOutData[i].Kind,
                        Use = returnInOutData[i].Use,
                        Bank = returnInOutData[i].Bank,
                        Card = returnInOutData[i].Card,
                        UseDate = returnInOutData[i].UseDate,
                        Detail = returnInOutData[i].Detail
                    });
                if (returnInOutData[i].InOut == "입금" ) inTotal += int.Parse(returnInOutData[i].Money!);
                else outTotal += int.Parse(returnInOutData[i].Money!);
            }

            OutTotalSum = String.Format("{0:#,###원}", outTotal);
            TotalSum = String.Format("{0:#,###원}", inTotal - outTotal);
        }

        private void LoadInOutView()
        {
            SelectedToDate = DateTime.Now;
            SelectedFromDate = DateTime.Now.AddDays(-30);

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

            List<Card> returnCard = dataBaseService.SelectCardCode();
            Card = new ObservableCollection<string>();
            for (int i = 0; i < returnCard.Count; i++)
            {
                if (returnCard[i].Description != "") Card.Add(returnCard[i].Description!);
            }
            Card.Add("전체");
            SelectedCard = "전체";
        
            List<Bank> returnBank = dataBaseService.SelectBankCode();
            Bank = new ObservableCollection<string>();
            for (int i = 0; i < returnBank.Count; i++)
            {
                if (!Bank.Contains(returnBank[i].Description!)) Bank.Add(returnBank[i].Description!);
            }
            Bank.Add("전체");
            SelectedBank = "전체";

            List<Use> returnUse = dataBaseService.SelectUseCode();
            Use = new ObservableCollection<string>();
            for (int i = 0; i < returnUse.Count; i++)
            {
                Use.Add(returnUse[i].Description!);
            }
            Use.Add("전체");
            SelectedUse = "전체";

            List<InOutModel> returnInOutData = dataBaseService.SelectAllInOutData(
                SelectedFromDate.ToString("yyyy-MM-dd"), SelectedToDate.ToString("yyyy-MM-dd"), 
                new InOutModel
                {
                    Bank = SelectedBank,
                    Card = SelectedCard,
                    Kind = SelectedKind,
                    Use = SelectedUse,
                    InOut = SelectedInOut
                });

            InOutList = new ObservableCollection<InOutModel>();

            int inTotal = 0;
            int outTotal = 0;

            for (int i = 0; i < returnInOutData.Count; i++)
            {
                InOutList.Add(
                    new InOutModel 
                    { 
                        InOut = returnInOutData[i].InOut, 
                        Money = String.Format("{0:#,###}", int.Parse(returnInOutData[i].Money!)),
                        Kind = returnInOutData[i].Kind,
                        Use = returnInOutData[i].Use,
                        Bank= returnInOutData[i].Bank,
                        Card = returnInOutData[i].Card,
                        UseDate = returnInOutData[i].UseDate,
                        Detail = returnInOutData[i].Detail
                    });

                if (returnInOutData[i].InOut == "입금") inTotal += int.Parse(returnInOutData[i].Money!);
                else outTotal += int.Parse(returnInOutData[i].Money!);
            }

            OutTotalSum = String.Format("{0:#,###원}", outTotal);
            TotalSum = String.Format("{0:#,###원}", inTotal - outTotal);
        }
        public void UnloadInOutView()
        {
            InOutList.Clear();
            InOut.Clear();
            Use.Clear();
            Bank.Clear();
            Card.Clear();
        }
        private void IsChangedKind()
        {
            if (SelectedKind == "전체")
            {
                List<Card> returnCard = dataBaseService.SelectCardCode();
                Card = new ObservableCollection<string>();
                for (int i = 0; i < returnCard.Count; i++)
                {
                    if (returnCard[i].Description != "") Card.Add(returnCard[i].Description!);
                }
                Card.Add("전체");
                SelectedCard = "전체";

                List<Bank> returnBank = dataBaseService.SelectBankCode();
                Bank = new ObservableCollection<string>();
                for (int i = 0; i < returnBank.Count; i++)
                {
                    if (!Bank.Contains(returnBank[i].Description!)) Bank.Add(returnBank[i].Description!);
                }
                Bank.Add("전체");
                SelectedBank = "전체";  
            }
            else
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
                            if (returnCard[k].Description == "") { }
                            else
                            {
                                if ((returnBank[i].Name == returnCard[k].Bank) && !Card.Contains(returnCard[k].Description!)) Card.Add(returnCard[k].Description!);
                            }

                        }
                    }
                }
                Bank.Add("전체");
                SelectedBank = "전체";

                if (!Card.Contains("전체")) Card.Add("전체");
                SelectedCard = "전체";
            }
        }
        private void IsChangedBank()
        {
            if (SelectedBank == "전체") 
            {
                List<Card> returnCard = dataBaseService.SelectCardCode();
                Card = new ObservableCollection<string>();
                for (int i = 0; i < returnCard.Count; i++)
                {
                    if (returnCard[i].Description != "") Card.Add(returnCard[i].Description!);
                }
                if (!Card.Contains("전체")) Card.Add("전체");
                SelectedCard = "전체";
            }
            else
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
                Card.Add("전체");
                SelectedCard = "전체";
            }  
        }
    }
}

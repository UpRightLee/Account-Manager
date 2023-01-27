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
    public class SummaryViewModel : INotifier
    {
        private static IDataBaseService dataBaseService = DataBaseService.Instance;

        private ObservableCollection<string> yearList = new ObservableCollection<string>();
        private ObservableCollection<SummaryData> summaryList = new ObservableCollection<SummaryData>();
        private ObservableCollection<SummaryData> yearSummaryList = new ObservableCollection<SummaryData>();

        private string totalSum = "";
        private string selectedYear = "";
        public ObservableCollection<SummaryData> YearSummaryList
        {
            get { return yearSummaryList; }
            set
            {
                yearSummaryList = value;
                OnPropertyChanged("YearSummaryList");
            }
        }
        public ObservableCollection<SummaryData> SummaryList
        {
            get { return summaryList; }
            set
            {
                summaryList = value;
                OnPropertyChanged("SummaryList");
            }
        }
        public ObservableCollection<string> YearList
        {
            get { return yearList; }
            set
            {
                yearList = value;
                OnPropertyChanged("YearList");
            }
        }
        public string SelectedYear
        {
            get { return selectedYear; }
            set
            {
                selectedYear = value;
                OnPropertyChanged("SelectedYear");
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
        public RelayCommand LoadSummaryViewCommand { get; }
        public RelayCommand SelectDataCommand { get; }
        public RelayCommand ExcelDownloadCommand { get; }
        public SummaryViewModel() 
        {
            LoadSummaryViewCommand = new RelayCommand(LoadSummaryView);
            SelectDataCommand = new RelayCommand(SelectData);
            ExcelDownloadCommand = new RelayCommand(ExcelDownload);
        }

        private void ExcelDownload()
        {
            Console.WriteLine("ExcelDownload Command");
        }
        private void SelectData()
        {
            List<SummaryData> returnData = dataBaseService.SelectBalanceInfo(SelectedYear);

            SummaryList = new ObservableCollection<SummaryData>();
            int isInMonthIndex = 0;
            bool isInMonth = false;
            bool isOutMonth = false;
            for (int i = 0; i < returnData.Count; i++)
            {
                if (returnData[i].InOut == "입금")
                {
                    SummaryList.Add(new SummaryData
                    {
                        Money = returnData[i].Money,
                        Month = returnData[i].Month,
                        Year = returnData[i].Year,
                        InOut = returnData[i].InOut
                    });
                    isInMonth = true;
                }
                else if (returnData[i].InOut == "출금")
                {
                    isInMonthIndex = i;
                    isOutMonth = true;
                }

                if (isInMonth & isOutMonth)
                {
                    for (int k = 0; k < SummaryList.Count; k++)
                    {
                        if (returnData[isInMonthIndex].Month == SummaryList[k].Month)
                        {
                            if (uint.Parse(SummaryList[k].Money!) > uint.Parse(returnData[isInMonthIndex].Money!))
                                SummaryList[k].Money = (uint.Parse(SummaryList[k].Money!) - uint.Parse(returnData[isInMonthIndex].Money!)).ToString();
                            else SummaryList[k].Money = "-" + (uint.Parse(returnData[isInMonthIndex].Money!) - uint.Parse(SummaryList[k].Money!)).ToString();
                        }
                    }
                    isInMonth = false;
                    isOutMonth = false;
                }
            }

            string inSummary = dataBaseService.SelectINBalanceInfo();
            string outSummary = dataBaseService.SelectOUTBalanceInfo();
            TotalSum = String.Format("{0:#,###}원", int.Parse(inSummary) - int.Parse(outSummary));

            for (int i = 0; i < SummaryList.Count; i++) SummaryList[i].Money = String.Format("{0:#,###원}", int.Parse(SummaryList[i].Money!));
        }
        private void LoadSummaryView()
        {
            YearList = new ObservableCollection<string>();
            List<int> years = Enumerable.Range(1950, DateTime.Today.Year).ToList();

            years.ForEach(year => YearList.Add(year.ToString()));
            SelectedYear = DateTime.Today.Year.ToString();

            SelectData();
        }
    }
}

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
                }
                if (returnData[i].InOut == "출금")
                {
                    if (i == 0)
                    {
                        SummaryList.Add(new SummaryData
                        {
                            Money = returnData[i].Money,
                            Month = returnData[i].Month,
                            Year = returnData[i].Year,
                            InOut = returnData[i].InOut
                        });
                    }
                    for (int k = 0; k < SummaryList.Count; k++)
                    {
                        if (returnData[i].Month == SummaryList[k].Month)
                        {
                            int sum = int.Parse(SummaryList[k].Money!) - int.Parse(returnData[i].Money!);
                            SummaryList[k].Money = sum.ToString();
                        }
                    }
                    
                }
            }
            YearSummaryList = new ObservableCollection<SummaryData>();
            for (int i = 0; i < SummaryList.Count; i++)
            {
                if (YearSummaryList.Count > 0)
                {
                    for (int k = 0; k < YearSummaryList.Count; k++)
                    {
                        if (SummaryList[i].Year == YearSummaryList[k].Year) YearSummaryList[k].Money = (YearSummaryList[k].returnInt(YearSummaryList[k].Money!) + SummaryList[i].returnInt(SummaryList[i].Money!)).ToString();
                    }

                    //YearSummaryList.Add(SummaryList[i]);
                }
                else
                {
                    YearSummaryList.Add(new SummaryData
                    {
                        Year = SummaryList[i].Year,
                        Money = SummaryList[i].Money
                    });
                }

                SummaryList[i].Money = String.Format("{0:#,###}", int.Parse(SummaryList[i].Money!));
            }
            if (YearSummaryList.Count > 0) yearSummaryList[0].Money = String.Format("{0:#,###}", int.Parse(yearSummaryList[0]?.Money!));
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

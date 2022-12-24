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
    public class SummaryViewModel : INotifier
    {
        private ObservableCollection<string> yearList = new ObservableCollection<string>();
        private ObservableCollection<string> monthList = new ObservableCollection<string>();
        
        private string selectedYear = "";
        private string selectedMonth = "";

        public ObservableCollection<string> YearList
        {
            get { return yearList; }
            set
            {
                yearList = value;
                OnPropertyChanged("YearList");
            }
        }
        public ObservableCollection<string> MonthList
        {
            get { return monthList; }
            set
            {
                monthList = value;
                OnPropertyChanged("MonthList");
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
        public string SelectedMonth
        {
            get { return selectedMonth; }
            set
            {
                selectedMonth = value;
                OnPropertyChanged("SelectedMonth");
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
            Console.WriteLine("Select Data Command");
        }
        private void LoadSummaryView()
        {
            YearList = new ObservableCollection<string>();
            List<int> years = Enumerable.Range(1950, DateTime.Today.Year).ToList();

            years.ForEach(year => YearList.Add(year.ToString()));
            SelectedYear = DateTime.Today.Year.ToString();

            MonthList = new ObservableCollection<string>();
            List<int> months = Enumerable.Range(1, 12).ToList();
            months.ForEach(month => MonthList.Add(month.ToString()));
            SelectedMonth = "전체";
        }
    }
}

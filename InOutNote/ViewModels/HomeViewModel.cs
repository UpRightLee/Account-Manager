using InOutNote.Command;
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
    public class HomeViewModel : INotifier
    {
        private ObservableCollection<WeeklyReport> inDataList = new ObservableCollection<WeeklyReport>();
        private ObservableCollection<WeeklyReport> outDataList = new ObservableCollection<WeeklyReport>();
        private ObservableCollection<MonthlyReport> inMonthDataList = new ObservableCollection<MonthlyReport>();
        private ObservableCollection<MonthlyReport> outMonthDataList = new ObservableCollection<MonthlyReport>();

        public ObservableCollection<WeeklyReport> InDataList
        {
            get 
            {
                return inDataList;
            }
            set
            {
                inDataList = value;
                OnPropertyChanged("InDataList");
            }
        }
        public ObservableCollection<WeeklyReport> OutDataList
        {
            get
            {
                return outDataList;
            }
            set
            {
                outDataList = value;
                OnPropertyChanged("OutDataList");
            }
        }
        public ObservableCollection<MonthlyReport> InMonthDataList
        {
            get
            {
                return inMonthDataList;
            }
            set
            {
                inMonthDataList = value;
                OnPropertyChanged("InMonthDataList");
            }
        }
        public ObservableCollection<MonthlyReport> OutMonthDataList
        {
            get
            {
                return outMonthDataList;
            }
            set
            {
                outMonthDataList = value;
                OnPropertyChanged("OutMonthDataList");
            }
        }
        public RelayCommand LoadHomeViewCommand { get; }
        public HomeViewModel()
        {
            LoadHomeViewCommand = new RelayCommand(LoadHomeView);          
        }

        private void LoadHomeView()
        {
            InDataList = new ObservableCollection<WeeklyReport>();
            InDataList.Add(new WeeklyReport { WeekDay = "월요일", Money = 10000 });
            InDataList.Add(new WeeklyReport { WeekDay = "화요일", Money = 8000 });
            InDataList.Add(new WeeklyReport { WeekDay = "수요일", Money = 7500 });
            InDataList.Add(new WeeklyReport { WeekDay = "목요일", Money = 25000 });
            InDataList.Add(new WeeklyReport { WeekDay = "금요일", Money = 32000 });
            InDataList.Add(new WeeklyReport { WeekDay = "토요일", Money = 7000 });
            InDataList.Add(new WeeklyReport { WeekDay = "일요일", Money = 5000 });

            OutDataList = new ObservableCollection<WeeklyReport>();
            OutDataList.Add(new WeeklyReport { WeekDay = "월요일", Money = 5000 });
            OutDataList.Add(new WeeklyReport { WeekDay = "화요일", Money = 6500 });
            OutDataList.Add(new WeeklyReport { WeekDay = "수요일", Money = 14000 });
            OutDataList.Add(new WeeklyReport { WeekDay = "목요일", Money = 22000 });
            OutDataList.Add(new WeeklyReport { WeekDay = "금요일", Money = 2000 });
            OutDataList.Add(new WeeklyReport { WeekDay = "토요일", Money = 9000 });
            OutDataList.Add(new WeeklyReport { WeekDay = "일요일", Money = 1000 });

            InMonthDataList = new ObservableCollection<MonthlyReport>();
            InMonthDataList.Add(new MonthlyReport { MonthsName = "1월", Money = 2500000 });
            InMonthDataList.Add(new MonthlyReport { MonthsName = "2월", Money = 2500000 });
            InMonthDataList.Add(new MonthlyReport { MonthsName = "3월", Money = 2500000 });
            InMonthDataList.Add(new MonthlyReport { MonthsName = "4월", Money = 2500000 });
            InMonthDataList.Add(new MonthlyReport { MonthsName = "5월", Money = 2500000 });

            OutMonthDataList = new ObservableCollection<MonthlyReport>();
            OutMonthDataList.Add(new MonthlyReport { MonthsName = "1월", Money = 520000 });
            OutMonthDataList.Add(new MonthlyReport { MonthsName = "2월", Money = 550000 });
            OutMonthDataList.Add(new MonthlyReport { MonthsName = "3월", Money = 580000 });
            OutMonthDataList.Add(new MonthlyReport { MonthsName = "4월", Money = 570000 });
            OutMonthDataList.Add(new MonthlyReport { MonthsName = "5월", Money = 450000 });
        }
    }
}

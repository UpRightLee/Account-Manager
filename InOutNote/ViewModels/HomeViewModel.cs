using InOutNote.Command;
using InOutNote.DataBase;
using InOutNote.Models;
using InOutNote.Notifier;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InOutNote.ViewModels
{
    public class HomeViewModel : INotifier
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HomeViewModel));

        private static IDataBaseService dataBaseService = DataBaseService.Instance;

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
            List<InOutModel> weeklyData = dataBaseService.SelectWeeklyInOutData();
            List<InOutModel> monthlyData = dataBaseService.SelectMonthlyInOutData();

            if (weeklyData.Count <= 0)
            {
                log.Error($"{MethodBase.GetCurrentMethod()?.Name}:: Weekly Data No Data Returned");
            }
            if (monthlyData.Count <= 0)
            {
                log.Error($"{MethodBase.GetCurrentMethod()?.Name}:: Monthly Data No Data Returned");
            }

            InDataList = new ObservableCollection<WeeklyReport>();
            OutDataList = new ObservableCollection<WeeklyReport>();

            for (int i = 0; i < weeklyData.Count; i++)
            {
                if (weeklyData[i].InOut == "입금")
                {
                    InDataList.Add(new WeeklyReport { WeekDay = weeklyData[i].UseDate, Money = weeklyData[i].returnInt(weeklyData[i]?.Money!) });
                }
                else if (weeklyData[i].InOut == "출금")
                {
                    OutDataList.Add(new WeeklyReport { WeekDay = weeklyData[i].UseDate, Money = weeklyData[i].returnInt(weeklyData[i]?.Money!) });
                }
            }

            InMonthDataList = new ObservableCollection<MonthlyReport>();
            OutMonthDataList = new ObservableCollection<MonthlyReport>();

            for (int i = 0; i < monthlyData.Count; i++)
            {
                if (monthlyData[i].InOut == "입금")
                {
                    InMonthDataList.Add(new MonthlyReport { MonthsName = monthlyData[i].UseDate?.Substring(5,2)+"월", Money = monthlyData[i].returnInt(monthlyData[i]?.Money!) });
                }
                else if (monthlyData[i].InOut == "출금")
                {
                    OutMonthDataList.Add(new MonthlyReport { MonthsName = monthlyData[i].UseDate?.Substring(5, 2) + "월", Money = monthlyData[i].returnInt(monthlyData[i]?.Money!) });
                }
            }
        }
    }
}

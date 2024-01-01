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
    public class GroupViewModel : INotifier
    {
        private DateTime selectedFromDate;
        private DateTime selectedToDate;

        private static IDataBaseService dataBaseService = DataBaseService.Instance;

        private ObservableCollection<InOutModel> groupList = new ObservableCollection<InOutModel>();

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
        public ObservableCollection<InOutModel> GroupList
        {
            get { return groupList; }
            set
            {
                groupList = value;
                OnPropertyChanged("GroupList");
            }
        }

        public RelayCommand LoadGroupViewCommand { get; }
        public RelayCommand UnloadGroupViewCommand { get; }
        public RelayCommand SelectDataCommand { get; }
        public GroupViewModel()
        {
            LoadGroupViewCommand = new RelayCommand(LoadGroupView);
            SelectDataCommand = new RelayCommand(SelectData);
            UnloadGroupViewCommand = new RelayCommand(UnloadGroupView);
        }

        private void UnloadGroupView()
        {
            GroupList.Clear();
        }

        private void LoadGroupView()
        {
            SelectedToDate = DateTime.Now;
            SelectedFromDate = DateTime.Now.AddMonths(-3);

            SelectData();
        }

        private void SelectData()
        {
            List<InOutModel> returnInOutData = dataBaseService.SelectGroupByInOutData(SelectedFromDate.ToString("yyyy-MM-dd"), SelectedToDate.ToString("yyyy-MM-dd"));

            GroupList = new ObservableCollection<InOutModel>();

            for (int i = 0; i < returnInOutData.Count; i++) 
            {
                GroupList.Add(new InOutModel
                {
                    UseDate = returnInOutData[i].UseDate + "월",
                    Money = String.Format("{0:#,###}", int.Parse(returnInOutData[i].Money!)),
                    Bank = returnInOutData[i].Bank,
                    Card = returnInOutData[i].Card,
                    Use = returnInOutData[i].Use
                });
            }
        }
    }
}

using InOutNote.DataBase;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace InOutNote
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {  
        private static readonly ILog log = LogManager.GetLogger(typeof(App));
        private static IDataBaseService dataBaseService = DataBaseService.Instance;
        protected override void OnStartup(StartupEventArgs e)
        {
            log.Info("=============Application Start=============");
            dataBaseService.CreateDB();
            base.OnStartup(e);
        }
    }
}

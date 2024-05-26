using AllTechnologyWpf.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AllTechnologyWpf
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static AllTechnologyWpfDatabaseEntities DB = new AllTechnologyWpfDatabaseEntities();
        public static Models.Color color;
        public static string EmailName = "bestdiaryrussian@gmail.com";
        public static string EmailPassword = "scnx oqpv qwxh ipwx";

        public App()
        {
            var currentDomain = AppDomain.CurrentDomain;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            MessageBox.Show("Error");
        }
    }
}

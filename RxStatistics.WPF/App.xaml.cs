using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Windows;

namespace RxStatistics.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Scheduler.Default.Catch<Exception>(DefaultScheduler_UnhandledException);
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            Trace.WriteLine(e.Exception);
        }

         static bool DefaultScheduler_UnhandledException(Exception arg)
        {
            Trace.WriteLine(arg);
            return true;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Trace.WriteLine(e.ExceptionObject);
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RxStatistics.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();


            var task = WhenClickOnce(this.Btn);
        }

        public static Task<RoutedEventArgs> WhenClickOnce(Button btn)
        {
            var tcs = new TaskCompletionSource<RoutedEventArgs>();
            RoutedEventHandler handler = null;
            handler = new RoutedEventHandler((s, e) =>
                {
                    tcs.SetResult(e);
                    btn.Click -= handler;
                });
            btn.Click += handler;
            return tcs.Task;
        }
    }
}

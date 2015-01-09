using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace RxStatistics.WPF
{
    public class MainViewModel : ReactiveObject
    {

        private readonly SimpleUsageViewModel _simpleVm;
        private readonly TimeUsageViewModel _timeVm;

        public MainViewModel()
        {
            _simpleVm = new SimpleUsageViewModel();
            _timeVm = new TimeUsageViewModel();
        }

        public TimeUsageViewModel TimeVm
        {
            get { return _timeVm; }
        }
        public SimpleUsageViewModel SimpleVm
        {
            get { return _simpleVm; }
        }
    }
}

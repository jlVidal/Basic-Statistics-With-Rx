using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace RxStatistics.WPF
{
    public class ConfigTimeViewModel : ReactiveObject
    {
        private ReactiveCommand<object> Start { get; set; }
        private ReactiveCommand<object> Stop { get; set; }
        private ReactiveCommand<object> Reset { get; set; }

        private decimal _minimum = 1M;
        private decimal _maximum = 5M;

        private bool _slow = true;
        private bool _fast;

        public bool Slow
        {
            get { return _slow; }
            set { this.RaiseAndSetIfChanged(ref _slow, value); }
        }
     
        public bool Fast
        {
            get { return _fast; }
            set { this.RaiseAndSetIfChanged(ref _fast, value); }
        }

        public ConfigTimeViewModel()
        {
            this.WhenAnyValue(a => a.Minimum)
                .Where(a => a > Maximum)
                .Do(a => Maximum = Minimum)
                .Subscribe();

            this.WhenAnyValue(a => a.Maximum)
                .Where(a => a < Minimum)
                .Do(a => Minimum = Maximum)
                .Subscribe();
               
        }

        public decimal Maximum
        {
            get { return _maximum; }
            set { this.RaiseAndSetIfChanged(ref _maximum, value); }
        }
        public decimal Minimum
        {
            get { return _minimum; }
            set { this.RaiseAndSetIfChanged(ref _minimum, value); }
        }


    }
}

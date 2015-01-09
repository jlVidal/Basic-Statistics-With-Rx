using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;

namespace RxStatistics.WPF
{
    public class DataServiceViewModel : ReactiveObject
    {
        private IObservable<decimal> _data;
        private readonly ConfigTimeViewModel _config;
        private bool _isRunning;
        public IObservable<decimal> Data
        {
            get { return _data; }
        }
        public ReactiveCommand<object> StartCommand { get; set; }
        public ReactiveCommand<object> StopCommand { get; set; }
        public bool IsRunning
        {
            get { return _isRunning; }
            set { this.RaiseAndSetIfChanged(ref _isRunning, value); }
        }
        public DataServiceViewModel(ConfigTimeViewModel config)
        {
            this._config = config;
            StartCommand = ReactiveCommand.Create(this.WhenAny(a => a.IsRunning, a => !a.Value));
            StopCommand = ReactiveCommand.Create(this.WhenAnyValue(a => a.IsRunning));

            var other = Observable.Create<decimal>(obs =>
                {
                    return StartCommand
                        .Do(a => this.IsRunning = true)
                        .Select(a => CreateSubscription(config.Slow, config.Minimum, config.Maximum))
                        .Switch()
                        .TakeUntil(StopCommand.Do(a => this.IsRunning = false))
                        .Repeat()
                     .Subscribe(obs);
                })
                .Publish();

            _data = other;
            other.Connect();
        }

        private IObservable<decimal> CreateSubscription(bool isSlow, decimal minimum, decimal maximum)
        {
            if (isSlow)
                return GenerateSamplePerSecond(1, 6, minimum, maximum);

            return GenerateSamplePerSecond(6, 21, minimum, maximum);
        }

        private IObservable<decimal> GenerateSamplePerSecond(int startTimes, int endTimes, decimal minimumValue, decimal maximumValue)
        {
            return Observable.Create<decimal>(obs =>
                {
                    var rand = new Random();
                    var perSecond = rand.Next(startTimes, endTimes);
                    var timeShift = 1000 / perSecond;

                    var disp = Scheduler.Default.Schedule(DateTime.Now, self =>
                    {
                        var now = DateTime.Now;
                        var value = rand.Next(Convert.ToInt32(Math.Min(Int32.MaxValue, minimumValue * 10)),
                                                Convert.ToInt32(Math.Min(Int32.MaxValue, maximumValue * 10 + 1)));

                        var nextValue = (decimal)value / 10;

                        var nextExecution = DateTime.Now.Ticks + TimeSpan.FromMilliseconds(timeShift).Ticks;
                        var nextDate = new DateTime(nextExecution);
                        obs.OnNext(nextValue);
                        self(nextDate);
                    });

                    return disp;
                });
        }

    }
}

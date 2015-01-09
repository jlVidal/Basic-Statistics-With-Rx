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
    public class SimpleUsageViewModel : ReactiveObject
    {
        private readonly ReactiveList<decimal> _sum = new ReactiveList<decimal>();

        private readonly ReactiveList<IGrouping<int, decimal>> _modaValue = new ReactiveList<IGrouping<int, decimal>>();
        private readonly ReactiveList<string> _modaText = new ReactiveList<string>();

        private readonly ReactiveList<decimal> _median = new ReactiveList<decimal>();
        private readonly ReactiveList<decimal> _average = new ReactiveList<decimal>();
        private readonly ReactiveList<decimal> _max = new ReactiveList<decimal>();
        private readonly ReactiveList<decimal> _min = new ReactiveList<decimal>();
        private readonly ReactiveList<int> _counter = new ReactiveList<int>();

        private decimal _value;
        private readonly ReactiveList<KeyValuePair<int, decimal>> _history = new ReactiveList<KeyValuePair<int, decimal>>();

        public SimpleUsageViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            InsertValueCommand = ReactiveCommand.Create();
            IObservable<decimal> addCommand = InsertValueCommand.Select(a => Value)
                                                .Publish().RefCount();

            ClearStatisticsCommand = ReactiveCommand.Create();
            IObservable<object> cleanMethod = ClearStatisticsCommand;

            addCommand.Select((a, b) => new KeyValuePair<int, decimal>(b + 1, a))
                        .TakeUntil(cleanMethod).Repeat()
                        .Do(a => _history.Insert(0, a))
                        .Subscribe();

            var liveCount = addCommand.LiveCount().TakeUntil(cleanMethod).Repeat();
            liveCount.Subscribe(a => _counter.Insert(0, a));

            var liveAverage = addCommand.LiveAverage().TakeUntil(cleanMethod).Repeat();
            liveAverage.Subscribe(a => _average.Insert(0, a));

            var liveSum = addCommand.LiveSum().TakeUntil(cleanMethod).Repeat();
            liveSum.Subscribe(a => _sum.Insert(0, a));

            var liveMax = addCommand.LiveMax().TakeUntil(cleanMethod).Repeat();
            liveMax.Subscribe(a => _max.Insert(0, a));

            var liveMin = addCommand.LiveMin().TakeUntil(cleanMethod).Repeat();
            liveMin.Subscribe(a => _min.Insert(0, a));

            var liveMedian = addCommand.LiveMedian().TakeUntil(cleanMethod).Repeat();
            liveMedian.Subscribe(a => _median.Insert(0, a));

            var liveModa = addCommand.LiveModa().TakeUntil(cleanMethod).Repeat();
            liveModa.Subscribe(a => _modaValue.Insert(0, a));

            var textModa = _modaValue.ItemsAdded.Select(a => a.Key + "x = " +
                                                         new string(
                                                         a.Select(b => string.Format("{0:C2}", b))
                                                             .Aggregate((b, c) => b + " & " + c)
                                                             .ToArray()
                                                         ));
            textModa.Subscribe(a => _modaText.Insert(0, a));

            cleanMethod.Do(a => _counter.Clear())
                        .Do(a => _average.Clear())
                        .Do(a => _sum.Clear())
                        .Do(a => _max.Clear())
                        .Do(a => _min.Clear())
                        .Do(a => _modaText.Clear())
                        .Do(a => _median.Clear())
                        .Do(a => _history.Clear())
                        .Subscribe();
        }

        public ReactiveCommand<object> InsertValueCommand { get; set; }

        public ReactiveCommand<object> ClearStatisticsCommand { get; set; }

        public decimal Value
        {
            get { return _value; }
            set { this.RaiseAndSetIfChanged(ref _value, value); }
        }

        public ReactiveList<int> Counter
        {
            get { return _counter; }
        }

        public ReactiveList<decimal> Min
        {
            get { return _min; }
        }

        public ReactiveList<decimal> Max
        {
            get { return _max; }
        }

        public ReactiveList<decimal> Average
        {
            get { return _average; }
        }

        public ReactiveList<decimal> Sum
        {
            get { return _sum; }
        }

        public ReactiveList<string> ModaText
        {
            get { return _modaText; }
        }

        public ReactiveList<decimal> Median
        {
            get { return _median; }
        }
        public ReactiveList<KeyValuePair<int, decimal>> Pairs
        {
            get { return _history; }
        }
    }
}

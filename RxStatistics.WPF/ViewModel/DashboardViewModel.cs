using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using RxStatistics.WPF.ViewModel;
using System.Diagnostics;

namespace RxStatistics.WPF
{
    public class DashboardViewModel : ReactiveObject
    {
        public const int TimeToReadData = 2000;

        private readonly DataServiceViewModel _dashboardVm;
        private readonly ReactiveList<PairViewModel<DateTime, double>> _chartValues;
        private IValueViewModel[] _toMonitor;

        public ReactiveList<PairViewModel<DateTime, double>> ChartValues
        {
            get { return _chartValues; }
        }

        public IValueViewModel[] MonitoredItems
        {
            get { return _toMonitor; }
            protected set { this.RaiseAndSetIfChanged(ref _toMonitor, value); }
        }

        public ReactiveCommand<object> ResetCommand { get; set; }
        public DashboardViewModel(DataServiceViewModel dashboardVm)
        {
            this._dashboardVm = dashboardVm;
            this._chartValues = new ReactiveList<PairViewModel<DateTime, double>>();

            Initialize();
        }

        private void Initialize()
        {
            _dashboardVm.StartCommand
                .Do(a => ResetData())
                .Select(next =>
                {
                    var dispChart = CreateChartSubscription();
                    var dispMonit = CreateMonitoredItemsSubscription();

                    return new[] { dispChart, dispMonit };
                })
                .Take(1)
                .Select(a => _dashboardVm.StopCommand.Select(b => a).Take(1))
                .Switch()
                .SelectMany(a => a.ToObservable())
                .Do(a => a.Dispose())
                .Repeat()
                .Subscribe();
        }

        private IDisposable CreateChartSubscription()
        {
            var compDisp = new System.Reactive.Disposables.CompositeDisposable();
            compDisp.Add(
                this.ChartValues.CountChanged.Where(a => a > 250)
                                              .Do(a => this._chartValues.RemoveAt(0))
                                              .Subscribe()
                                          );

            compDisp.Add(
                    this._dashboardVm
                        .Data
                        .Window(TimeSpan.FromSeconds(1))
                        .Select(a => a.Sum().Timestamp())
                        .Switch()
                        .ObserveOnDispatcher()
                        .Do(a => this.ChartValues.Add(new PairViewModel<DateTime, double>
                                {
                                    Key = a.Timestamp.LocalDateTime,
                                    Value = Convert.ToDouble(a.Value),
                                    Format = "N2"
                                })
                            )
                        .Subscribe()
                        );

            return compDisp;
        }

        private IDisposable CreateMonitoredItemsSubscription()
        {
            var itemsToMonitor = new List<IValueViewModel>();
            var compDisp = new System.Reactive.Disposables.CompositeDisposable();

            var totalIncome = new PairViewModel<string, decimal> { Key = "Total Income", Format = "C2" };
            compDisp.Add(CreateTotalIncomeSubs(totalIncome));
            itemsToMonitor.Add(totalIncome);

            var incomeAveragePerMinute = new PairViewModel<string, decimal> { Key = "Average / Minute", Format = "C2" };
            compDisp.Add(CreateAveragePerMinuteSubs(incomeAveragePerMinute));
            itemsToMonitor.Add(incomeAveragePerMinute);

            var modaInfo = new ModaInfoViewModel { Key = "Popular Sale", Format = "C2" };
            compDisp.Add(CreatePopularSaleSubs(modaInfo));
            itemsToMonitor.Add(modaInfo);

            var now = DateTime.Now;
            var timeShift = new PairViewModel<string, TimeSpan> { Key = "Time Monitored", Format = "hh\\:mm\\:ss" };
            compDisp.Add(CreateTimeMonitoredSubs(now, timeShift));
            itemsToMonitor.Add(timeShift);

            this.MonitoredItems = itemsToMonitor.ToArray();

            return compDisp;
        }

        private IDisposable CreateAveragePerMinuteSubs(PairViewModel<string, decimal> incomeAveragePerMinute)
        {
            var compDisp = new System.Reactive.Disposables.CompositeDisposable();

            var sx = this._dashboardVm.Data.Window(Observable.Defer(() =>
            {
                var rest = 60 - DateTime.Now.Second;
                return Observable.Timer(TimeSpan.FromSeconds(rest));
            }).Repeat()).Publish();

            compDisp.Add(
                sx.Select(a => a.Sum())
                    .Merge()
                    .LiveAverage()
                    .Merge(sx.Take(1)
                                .Select(a => a.LiveSum())
                                .Switch()
                                .Sample(TimeSpan.FromMilliseconds(TimeToReadData)))
                    .DistinctUntilChanged()
                    .ObserveOnDispatcher()
                    .Do(a => incomeAveragePerMinute.Value = a)
                    .Subscribe()
                        );

            compDisp.Add(sx.Connect());
            return compDisp;
        }

        private static IDisposable CreateTimeMonitoredSubs(DateTime now, PairViewModel<string, TimeSpan> timeShift)
        {
            return Observable.Timer(DateTime.Now, TimeSpan.FromSeconds(1))
                                         .Timestamp()
                                         .ObserveOnDispatcher()
                                         .Do(a => timeShift.Value = a.Timestamp - now)
                                         .Subscribe();
        }

        private IDisposable CreatePopularSaleSubs(ModaInfoViewModel modaInfo)
        {
            return this._dashboardVm.Data.LiveModa()
                        .DistinctUntilChanged(a => a.First())
                        .Sample(TimeSpan.FromMilliseconds(TimeToReadData))
                        .ObserveOnDispatcher()
                        .Do(a => modaInfo.Populate(a))
                        .Subscribe();
        }

        private IDisposable CreateTotalIncomeSubs(PairViewModel<string, decimal> totalIncomeVm)
        {
            return this._dashboardVm.Data.LiveSum()
                                              .DistinctUntilChanged()
                                              .Sample(TimeSpan.FromMilliseconds(TimeToReadData))
                                              .ObserveOnDispatcher()
                                              .Do(a => totalIncomeVm.Value = a)
                                              .Subscribe();
        }

        private void ResetData()
        {
            ChartValues.Clear();
            if (MonitoredItems == null)
                return;

            foreach (var item in MonitoredItems)
            {
                if (item.ValueObject == null)
                    return;

                item.Reset();
            }
        }
    }
}

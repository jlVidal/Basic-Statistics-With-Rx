using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Linq;
using System.Collections.Immutable;

namespace RxStatistics.WPF
{
    public class TimeUsageViewModel : ReactiveObject
    {
        private bool _isLoaded;
        private bool _isActive;
        private readonly ConfigTimeViewModel _configVm;
        private readonly DashboardViewModel _dashboardVm;
        private readonly DataServiceViewModel _dataServiceVm;

        public DashboardViewModel DashboardVm
        {
            get { return _dashboardVm; }
        } 
        public DataServiceViewModel DataServiceVm
        {
            get { return _dataServiceVm; }
        } 

        public bool IsActive
        {
            get { return _isActive; }
            set { this.RaiseAndSetIfChanged(ref _isActive, value); }
        }

        public bool IsLoaded
        {
            get { return _isLoaded; }
            set { this.RaiseAndSetIfChanged(ref _isLoaded, value); }
        }
        public ConfigTimeViewModel ConfigVm
        {
            get { return _configVm; }
        }
        public ReactiveCommand<object> LoadedCommand { get; protected set; }

        public TimeUsageViewModel()
        {
            _configVm = new ConfigTimeViewModel();
            _dataServiceVm = new DataServiceViewModel(_configVm);
            _dashboardVm = new DashboardViewModel(_dataServiceVm);

            Initialize();
        }

        private void Initialize()
        {
            LoadedCommand = ReactiveCommand.Create();
            LoadedCommand.Where(a => IsActive)
                            .Do(a => IsLoaded = true)
                            .Take(1)
                            .Subscribe();

            this.WhenAnyValue(a => a.IsLoaded)
                .Where(a => a)
                .Do(a => _dataServiceVm.StartCommand.Execute(null))
                .Take(1)
                .Subscribe();
        }
  

    }
}

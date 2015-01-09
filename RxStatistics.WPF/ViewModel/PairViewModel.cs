using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace RxStatistics
{
    public class PairViewModel<K, V> : ReactiveObject, RxStatistics.WPF.IValueViewModel
    {
        public PairViewModel()
        {
            this.WhenAnyValue(a => a.Value)
                .Do(a => this.RaisePropertyChanged("ValueObject"))
                .Subscribe();
        }

        public string Format { get; set; }

        private K _key;

        public virtual K Key
        {
            get { return _key; }
            set { this.RaiseAndSetIfChanged(ref _key, value); }
        }

        private V _value;

        public virtual V Value
        {
            get { return _value; }
            set
            {
                this.RaiseAndSetIfChanged(ref _value, value);
            }
        }

        public virtual object ValueObject
        {
            get { return Value; }
        }


        public virtual void Reset()
        {
            if (ValueObject == null)
                return;

            Value = Activator.CreateInstance<V>();
        }
    }
}

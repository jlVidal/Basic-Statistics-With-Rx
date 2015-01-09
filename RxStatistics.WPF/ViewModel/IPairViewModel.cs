using System;
namespace RxStatistics.WPF
{
    public interface IValueViewModel
    {
        object ValueObject { get;  }

        string Format { get; }

        void Reset();
    }
}

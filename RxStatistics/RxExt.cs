using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Collections.Immutable;
using System.Threading;

namespace RxStatistics
{
    public static class RxExt
    {
        public static IObservable<int> LiveCount<T>(this IObservable<T> source)
        {
            return source.Select((a, b) => b + 1);
        }
        public static IObservable<long> LiveLongCount<T>(this IObservable<T> source)
        {
            return source.Scan(0L, (a, b) => a + 1);
        }

        public static IObservable<int> LiveMin(this IObservable<int> source)
        {
            return source.Scan(Int32.MaxValue, Math.Min);
        }

        public static IObservable<double> LiveMin(this IObservable<double> source)
        {
            return source.Scan(double.MaxValue, Math.Min);
        }

        public static IObservable<decimal> LiveMin(this IObservable<decimal> source)
        {
            return source.Scan(decimal.MaxValue, Math.Min);
        }

        public static IObservable<int> LiveMax(this IObservable<int> source)
        {
            return source.Scan(Int32.MinValue, Math.Max);
        }

        public static IObservable<double> LiveMax(this IObservable<double> source)
        {
            return source.Scan(double.MinValue, Math.Max)
                        ;
        }

        public static IObservable<decimal> LiveMax(this IObservable<decimal> source)
        {
            return source.Scan(decimal.MinValue, Math.Max)
                        ;
        }

        public static IObservable<int> LiveSum(this IObservable<int> source)
        {
            return source.Scan(0, (a, b) => a + b)
                        ;
        }

        public static IObservable<decimal> LiveSum(this IObservable<decimal> source)
        {
            return source.Scan(0m, (a, b) => a + b)
                        ;
        }
        public static IObservable<double> LiveSum(this IObservable<double> source)
        {
            return source.Scan(0d, (a, b) => a + b)
                        ;
        }
        public static IObservable<int> LiveSum<T>(this IObservable<T> source, Func<T, int> func)
        {
            return source.Scan(0, (a, b) => a + func(b))
                        ;
        }

        public static IObservable<double> LiveSum<T>(this IObservable<T> source, Func<T, double> func)
        {
            return source.Scan(0D, (a, b) => a + func(b))
                        ;
        }

        public static IObservable<decimal> LiveSum<T>(this IObservable<T> source, Func<T, decimal> func)
        {
            return source.Scan(0m, (a, b) => a + func(b))
                        ;
        }

        public static IObservable<double> LiveAverage(this IObservable<int> source)
        {
            return source.Publish(p => p.LiveCount()
                                         .Zip(p.LiveSum(), (a, b) => (double)b / a));
        }

        public static IObservable<double> LiveAverage(this IObservable<double> source)
        {
            return source.Publish(p => p.LiveCount()
                                    .Zip(p.LiveSum(), (a, b) => b / a))
                        ;
        }
        public static IObservable<decimal> LiveAverage(this IObservable<decimal> source)
        {
            return source.Publish(p => p.LiveCount()
                                    .Zip(p.LiveSum(), (a, b) => b / a))
                        ;
        }

        public static IObservable<double> LiveAverage<T>(this IObservable<T> source, Func<T, int> func)
        {
            return source.Publish(p => p.LiveCount()
                                        .Zip(p.LiveSum(func), (a, b) => (double)b / a))
                        ;
        }

        public static IObservable<decimal> LiveAverage<T>(this IObservable<T> source, Func<T, decimal> func)
        {
            return source.Publish(p => p.LiveCount()
                                        .Zip(p.LiveSum(func), (a, b) => (decimal)b / a))
                        ;
        }

        public static IObservable<double> LiveAverage<T>(this IObservable<T> source, Func<T, double> func)
        {
            return source.Publish(p => p.LiveCount()
                                        .Zip(p.LiveSum(func), (a, b) => b / a))
                        ;
        }

        public static IObservable<IGrouping<int, int>> LiveModa(this IObservable<int> source)
        {
            return source.GroupBy(a => a)
                      .Select(a => a.LiveCount().Select(b => Tuple.Create(a.Key, b)))
                      .Merge()
                      .Scan(default(KeyValuePair<int, ImmutableStack<int>>), (a, b) =>
                      {
                          if (a.Key > b.Item2)
                              return a;

                          if (a.Key == b.Item2)
                              return new KeyValuePair<int, ImmutableStack<int>>(b.Item2, a.Value.Push(b.Item1));

                          return new KeyValuePair<int, ImmutableStack<int>>(b.Item2, ImmutableStack<int>.Empty.Push(b.Item1));
                      })
                      .Select(a => a.Value.AsGroup(a.Key))
                          ;
        }
        public static IObservable<IGrouping<int, decimal>> LiveModa(this IObservable<decimal> source)
        {
            return source.GroupBy(a => a)
                      .Select(a => a.LiveCount().Select(b => Tuple.Create(a.Key, b)))
                      .Merge()
                      .Scan(default(KeyValuePair<int, ImmutableStack<decimal>>), (a, b) =>
                      {
                          if (a.Key > b.Item2)
                              return a;

                          if (a.Key == b.Item2)
                              return new KeyValuePair<int, ImmutableStack<decimal>>(b.Item2, a.Value.Push(b.Item1));

                          return new KeyValuePair<int, ImmutableStack<decimal>>(b.Item2, ImmutableStack<decimal>.Empty.Push(b.Item1));
                      })
                      .Select(a => a.Value.AsGroup(a.Key))
                          ;
        }
        public static IObservable<int> LiveRange(this IObservable<int> source)
        {
            return source.Publish(p => p.LiveMax().Zip(p.LiveMin(), (a, b) => a - b).Skip(1))
                            ;
        }

        public static IObservable<double> LiveRange(this IObservable<double> source)
        {
            return source.Publish(p => p.LiveMax().Zip(p.LiveMin(), (a, b) => a - b).Skip(1))
                            ;
        }

        public static IObservable<decimal> LiveRange(this IObservable<decimal> source)
        {
            return source.Publish(p => p.LiveMax().Zip(p.LiveMin(), (a, b) => a - b).Skip(1))
                            ;
        }

        public static IObservable<double> LiveMedian(this IObservable<int> source)
        {
            return source.AccumulateAllOrdered()
                         .Select(a => (a.Count % 2) == 0
                                        ? (double)(a[(a.Count / 2) - 1] + a[a.Count / 2]) / 2
                                        : (double)(a[(a.Count / 2)])
                                )
                          ;
        }


        public static IObservable<decimal> LiveMedian(this IObservable<decimal> source)
        {
            return source.AccumulateAllOrdered()
                         .Select(a => (a.Count % 2) == 0
                                        ? (a[(a.Count / 2) - 1] + a[a.Count / 2]) / 2
                                        : (a[(a.Count / 2)])
                                )
                          ;
        }


        public static IObservable<double> LiveMedian(this IObservable<double> source)
        {
            return source.AccumulateAllOrdered()
                         .Select(a => (a.Count % 2) == 0
                                        ? (a[(a.Count / 2) - 1] + a[a.Count / 2]) / 2
                                        : (a[(a.Count / 2)])
                                )
                          ;
        }

        public static IObservable<IReadOnlyList<T>> AccumulateAll<T>(this IObservable<T> next)
        {
            return next.Scan(ImmutableList<T>.Empty, (a, b) => a.Add(b));
        }

        public static IObservable<IReadOnlyList<int>> AccumulateAllOrdered(this IObservable<int> source)
        {
            return source.Scan(ImmutableList<int>.Empty, (a, b) =>
            {
                var res = a.BinarySearch(b);
                if (res <= -1)
                    res = ~res;

                return a.Insert(res, b);
            });

        }
        public static IObservable<IReadOnlyList<double>> AccumulateAllOrdered(this IObservable<double> source)
        {
            return source.Scan(ImmutableList<double>.Empty, (a, b) =>
            {
                var res = a.BinarySearch(b);
                if (res <= -1)
                    res = ~res;

                return a.Insert(res, b);
            });
        }

        public static IObservable<IReadOnlyList<decimal>> AccumulateAllOrdered(this IObservable<decimal> source)
        {
            return source.Scan(ImmutableList<decimal>.Empty, (a, b) =>
            {
                var res = a.BinarySearch(b);
                if (res <= -1)
                    res = ~res;
               
                return a.Insert(res, b);
            });
        }


    }
}

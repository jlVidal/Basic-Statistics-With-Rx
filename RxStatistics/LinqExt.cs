using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxStatistics
{
    public static class LinqExt
    {
        public static IGrouping<K,V> AsGroup<K,V>(this IEnumerable<V> source, K key)
        {
            return SimpleGroupWrapper.Create(key, source); 
        }
    }

    internal static class SimpleGroupWrapper
    {
        public static SimpleGroupWrapper<K,V> Create<K,V>(K key, IEnumerable<V> source)
        {
            return new SimpleGroupWrapper<K, V>(key, source);
        }
    }
    internal class SimpleGroupWrapper<K,V> : IGrouping<K,V>
    {
        private readonly IEnumerable<V> _source;
        private readonly K _key;
        public SimpleGroupWrapper(K key, IEnumerable<V> source)
        {
            if (source == null)
                throw new NullReferenceException("source");

            _source = source;
            _key = key;
        }
        public K Key
        {
            get { return _key; }
        }

        public IEnumerator<V> GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _source.GetEnumerator();
        }
    }
}

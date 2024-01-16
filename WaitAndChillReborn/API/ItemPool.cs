using System;
using System.Collections.Generic;

namespace WaitAndChillReborn.API
{
    public static class ItemPool
    {
        public static ItemPool<T> ToPool<T>(this IEnumerable<T> values)
        {
            return new ItemPool<T>(values);
        }
    }

    public class ItemPool<T>
    {
        public List<T> Values { get; } = new();

        int index = 0;

        public ItemPool()
        {
        }

        public ItemPool(IEnumerable<T> values)
        {
            Values.AddRange(values);
        }

        public void Add(T value)
        {
            Values.Add(value);
        }

        public void AddRange(IEnumerable<T> values)
        {
            Values.AddRange(values);
        }

        public void ShuffleList()
        {
            Values.ShuffleList();
            index = 0;
        }

        public int Count => Values.Count;

        public void Clear() { Values.Clear(); index = 0; }

        public T GetNext()
        {
            if (index < Values.Count)
            {
                return Values[index++];
            }
            index = 0;
            return Values[index++];
        }

        public T GetNext(Func<T, bool> predicate)
        {
            int startIndex = index;
            while (true)
            {
                T next = GetNext();
                if (predicate(next)) return next;
                if (index == startIndex) return default;
            }
        }

        public static implicit operator ItemPool<T> (List<T> pool)
        {
            return ItemPool.ToPool(pool);
        }
        public static implicit operator ItemPool<T>(T[] pool)
        {
            return ItemPool.ToPool(pool);
        }
    }
}

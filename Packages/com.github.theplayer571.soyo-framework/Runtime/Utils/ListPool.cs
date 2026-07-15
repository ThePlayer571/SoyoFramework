using System.Collections.Generic;

namespace SoyoFramework.Utils
{
    internal static class ListPool<T>
    {
        private const int MaxPoolSize = 8;
        private static readonly Stack<List<T>> _pool = new(MaxPoolSize);

        internal static List<T> Rent()
        {
            return _pool.Count > 0 ? _pool.Pop() : new List<T>();
        }

        internal static void Return(List<T> list)
        {
            list.Clear();
            if (_pool.Count < MaxPoolSize)
            {
                _pool.Push(list);
            }
        }
    }
}

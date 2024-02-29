namespace WindowsCoreAudio
{
    using System;
    using System.Collections.Generic;

    public class EqualityComparer<T> : IEqualityComparer<T>
    {
        public Func<T, T, bool> Comparer { get; set; }

        public EqualityComparer(Func<T, T, bool> comparer)
        {
            this.Comparer = comparer;
        }

        public bool Equals(T x, T y)
        {
            return this.Comparer(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}

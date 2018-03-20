
using System;

namespace Checkbox.Common
{
    /// <summary>
    /// Value pair, useful for hashing with compound key. NULL NOT SUPPORTED FOR VALUES.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    [Serializable]
    public class Pair<T, V> : IEquatable<Pair<T, V>>
    {
        private int? _hashCode;

        /// <summary>
        /// Construct a pair
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        public Pair(T value1, V value2)
        {
            Value1 = value1;
            Value2 = value2;
        }

        /// <summary>
        /// Pair value 1
        /// </summary>
        public T Value1 { get; private set; }

        /// <summary>
        /// Pair value 2
        /// </summary>
        public V Value2 { get; private set; }

        /// <summary>
        /// Get object hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (!_hashCode.HasValue)
            {
                _hashCode = Value1.GetHashCode() ^ Value2.GetHashCode();
            }

            return _hashCode.Value;
        }

        /// <summary>
        /// Compare values
        /// </summary>
        /// <param name="toCompare"></param>
        /// <returns></returns>
        public bool Equals(Pair<T, V> toCompare)
        {
            return toCompare.Value1.Equals(Value1) && toCompare.Value2.Equals(Value2);
        }
    }
}
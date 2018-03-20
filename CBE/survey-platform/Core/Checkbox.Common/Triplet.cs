using System;

namespace Checkbox.Common
{
    /// <summary>
    /// Value triplet, useful for hashing with compound key. NULL NOT SUPPORTED FOR VALUES.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="U"></typeparam>
    [Serializable]
    public class Triplet<T, U, V> : IEquatable<Triplet<T, U, V>>
    {
        private int? _hashCode;

        /// <summary>
        /// Value 1
        /// </summary>
        public T Value1 { get; private set; }

        /// <summary>
        /// Value 2
        /// </summary>
        public U Value2 { get; private set; }

        /// <summary>
        /// Value 3
        /// </summary>
        public V Value3 { get; private set; }

        /// <summary>
        /// Construct a triplet
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="value3"></param>
        public Triplet(T value1, U value2, V value3)
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (!_hashCode.HasValue)
            {
                _hashCode = Value1.GetHashCode() ^ Value2.GetHashCode() ^ Value3.GetHashCode();
            }

            return _hashCode.Value;
        }

        /// <summary>
        /// Equality
        /// </summary>
        /// <param name="toCompare"></param>
        /// <returns></returns>
        public bool Equals(Triplet<T, U, V> toCompare)
        {
            return toCompare.Value1.Equals(Value1) && toCompare.Value2.Equals(Value2) && toCompare.Value3.Equals(Value3);
        }
    }
}

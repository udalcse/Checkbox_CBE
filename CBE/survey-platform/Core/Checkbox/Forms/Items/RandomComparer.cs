using System;
using System.Collections.Generic;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Comparer that provides a random +1 or -1 value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>The underlying "Random" object is static so to avoid potential issues with creating
    /// multiple "Random" objects concurrently that will product the same pseudorandom sequence.
    /// </remarks>
    public class RandomComparer<T> : IComparer<T>
    {
        private static readonly Random _random;

        /// <summary>
        /// Statid co
        /// </summary>
        static RandomComparer()
        {
            _random = new Random();
        }

        /// <summary>
        /// Get the next int
        /// </summary>
        /// <param name="minValue">Inclusive lower bound for value to return.</param>
        /// <param name="maxValue">Inclusive upper bound for value to return.</param>
        /// <returns>Random int between the two inclusive bounds.</returns>
        protected static int RandomGetNext(int minValue, int maxValue)
        {
            //The underlying random objects min value is inclusive, but the upper bound
            // is exclusive, so add 1 to the passed in parameter since this method returns
            // a random value with inclusive upper and lower bounds.
            return _random.Next(minValue, maxValue + 1);
        }

        #region IComparer<T> Members


        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(T x, T y)
        {
            //Return equals if values are equivalent
            if (x.Equals(y))
            {
                return 0;
            }
            
            //Otherwise, randomly return larger or smaller
            if (RandomGetNext(1, 2) == 1)
            {
                return 1;
            }
            
            return -1;
        }

        #endregion
    }
}

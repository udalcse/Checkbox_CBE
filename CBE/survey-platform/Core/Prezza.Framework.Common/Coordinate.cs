using System;
using System.Collections.Generic;

namespace Prezza.Framework.Common
{
    /// <summary>
    /// Simple class for containing coordinates
    /// </summary>
    [Serializable]
    public class Coordinate : IEquatable<Coordinate>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// X position
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Y Position
        /// </summary>
        public int Y { get; private set; }

        #region IEquatable<Coordinate> Members

        /// <summary>
        /// Compare this coordinate to the specified coordinate.  Returns true if coordinate x and y values match.
        /// </summary>
        /// <param name="other">Coordinate to compare.</param>
        /// <returns>Boolean indicating equality of the coordinates.</returns>
        public bool Equals(Coordinate other)
        {
            return (other.X == X && other.Y == Y);
        }

        #endregion
    }

    /// <summary>
    ///  Equality comparer for two coordinates
    /// </summary>
    [Serializable()]
    public class CoordinateComparer : IEqualityComparer<Coordinate>
    {
        #region IEqualityComparer<Coordinate> Members

        /// <summary>
        /// Determine if two coordinates are equal
        /// </summary>
        /// <param name="coordinate1"></param>
        /// <param name="coordinate2"></param>
        /// <returns></returns>
        public bool Equals(Coordinate coordinate1, Coordinate coordinate2)
        {
            return (coordinate1.X == coordinate2.X && coordinate1.Y == coordinate2.Y);
        }

        /// <summary>
        /// Get the hash code for a coordinate
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(Coordinate obj)
        {
            return obj.X * 100000 + obj.Y;
        }

        #endregion
    }
}

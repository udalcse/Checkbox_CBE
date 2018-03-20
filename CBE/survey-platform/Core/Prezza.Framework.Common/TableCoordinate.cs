using System;
using System.Collections.Generic;

namespace Prezza.Framework.Common
{
    /// <summary>
    /// Row/column class that supports comparision for use in storing table positions.
    /// </summary>
    [Serializable]
    public class TableCoordinate : IEquatable<TableCoordinate>
    {
        /// <summary>
        /// Construct a table coordinate
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public TableCoordinate(int row, int column)
        {
            Row = row;
            Column = column;
        }

        /// <summary>
        /// Get row position
        /// </summary>
        public int Row{get;private set;}

        /// <summary>
        /// Get column position
        /// </summary>
        public int Column{get;private set;}

        #region IEquatable<Coordinate> Members

        /// <summary>
        /// Compare this coordinate to the specified coordinate.  Returns true if coordinate x and y values match.
        /// </summary>
        /// <param name="other">Coordinate to compare.</param>
        /// <returns>Boolean indicating equality of the coordinates.</returns>
        public bool Equals(TableCoordinate other)
        {
            return (other.Row == Row && other.Column == other.Column);
        }

        #endregion
    }

    /// <summary>
    ///  Equality comparer for two coordinates
    /// </summary>
    [Serializable]
    public class TableCoordinateComparer : IEqualityComparer<TableCoordinate>
    {
        #region IEqualityComparer<Coordinate> Members

        /// <summary>
        /// Determine if two coordinates are equal
        /// </summary>
        /// <param name="coordinate1"></param>
        /// <param name="coordinate2"></param>
        /// <returns></returns>
        public bool Equals(TableCoordinate coordinate1, TableCoordinate coordinate2)
        {
            return (coordinate1.Row == coordinate2.Row && coordinate1.Column == coordinate2.Column);
        }

        /// <summary>
        /// Get the hash code for a coordinate
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(TableCoordinate obj)
        {
            return obj.Row * 100000 + obj.Column;
        }

        #endregion
    }
}

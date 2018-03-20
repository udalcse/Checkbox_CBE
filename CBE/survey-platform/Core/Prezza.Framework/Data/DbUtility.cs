//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Prezza.Framework.Data
{
    /// <summary>
    /// Various database utility routines that don't fit in their own classes.
    /// </summary>
    public static class DbUtility
    {
        private static DateTime? _tempImageCleanTime;

        /// <summary>
        /// Get the names of columns in a given table.
        /// </summary>
        /// <param name="tableName">Table to get column names from.f</param>
        /// <returns>String array with column names.</returns>
        public static string[] GetTableColumnNames(string tableName)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetSqlStringCommandWrapper("select top 1 * from " + tableName);

            DataColumnCollection cols = db.ExecuteDataSet(command).Tables[0].Columns;
            var colNames = new string[cols.Count];
            for(int i = 0; i < cols.Count; i++)
            {
                colNames[i] = cols[i].ColumnName;
            }
            return colNames;
        }

        /// <summary>
        /// Delete an image with the specified id
        /// </summary>
        /// <param name="imageID"></param>
        public static void DeleteImage(Int32 imageID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Image_DeleteImage");
            command.AddInParameter("ImageID", DbType.Int32, imageID);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Delete temporary images more than 1 hour old
        /// </summary>
        public static void DeleteTempImages()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Image_DeleteTempImages");
            command.AddInParameter("MaxDate", DbType.DateTime, DateTime.Now.Subtract(new TimeSpan(0, 1, 0, 0)));

            db.ExecuteNonQuery(command);
        }

         /// <summary>
        /// Save the specified image information to the database.
        /// </summary>
        /// <param name="file">Image file name.</param>
        /// <param name="contentType">Image content type.</param>
        /// <param name="imageUrl">URL of image.</param>
        /// <param name="imageName">Name of image.</param>
        /// <param name="guid">GUID to associate with the image.</param>
        /// <returns>Database ID associated with image.</returns>
        public static int SaveImage(byte[] file, string contentType, string imageUrl, string imageName, string guid)
        {
            return SaveImage(file, contentType, imageUrl, imageName, guid, DateTime.Now, false);
        }

        /// <summary>
        /// Save the specified image information to the database.
        /// </summary>
        /// <param name="file">Image file name.</param>
        /// <param name="contentType">Image content type.</param>
        /// <param name="imageUrl">URL of image.</param>
        /// <param name="imageName">Name of image.</param>
        /// <param name="guid">GUID to associate with the image.</param>
        /// <param name="createdDate">Created date for the image.</param>
        /// <param name="isTemporary">Indicates if the image is temporary.</param>
        /// <returns>Database ID associated with image.</returns>
        public static int SaveImage(byte[] file, string contentType, string imageUrl, string imageName, string guid, DateTime createdDate, bool isTemporary)
        {
            //Clean images if necessary
            if (!_tempImageCleanTime.HasValue || DateTime.Now.Subtract(new TimeSpan(1, 0, 0)) > _tempImageCleanTime.Value)
            {
                DeleteTempImages();
                _tempImageCleanTime = DateTime.Now;
            }

            if (string.IsNullOrEmpty(contentType))
            {
                contentType = GetImageContentType(imageName);
            }
         
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Image_InsertImage");
            command.AddInParameter("File", DbType.Binary, file);
            command.AddInParameter("ContentType", DbType.String, contentType);
            command.AddInParameter("ImageUrl", DbType.String, imageUrl);
            command.AddInParameter("ImageName", DbType.String, imageName);
            command.AddInParameter("Guid", DbType.String, guid);
            command.AddInParameter("CreatedDate", DbType.DateTime, createdDate);
            command.AddInParameter("IsTemporary", DbType.Boolean, isTemporary);
            command.AddOutParameter("ImageID", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            object id = command.GetParameterValue("ImageID");

            if (id == null || id == DBNull.Value)
            {
                return -1;
            }
            
            return (int)id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        private static string GetImageContentType(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
            {
                return "Unknown";
            }

            if (imageName.EndsWith("gif", StringComparison.InvariantCultureIgnoreCase))
            {
                return "image/gif";
            }

            if (imageName.EndsWith("bmp", StringComparison.InvariantCultureIgnoreCase))
            {
                return "image/bmp";
            }

            if (imageName.EndsWith("png", StringComparison.InvariantCultureIgnoreCase))
            {
                return "image/png";
            }

            if(imageName.EndsWith("jpg", StringComparison.InvariantCultureIgnoreCase)
                || imageName.EndsWith("jpeg", StringComparison.InvariantCultureIgnoreCase)
                || imageName.EndsWith("jpe", StringComparison.InvariantCultureIgnoreCase))
            {
                return "image/jpeg";
            }

            if (imageName.EndsWith("tif", StringComparison.InvariantCultureIgnoreCase)
               || imageName.EndsWith("tiff", StringComparison.InvariantCultureIgnoreCase))
            {
                return "image/tiff";
            }

            return "Unknown";
        }

        /// <summary>
        /// Get a data reader with information about the specified image.
        /// </summary>
        /// <param name="imageID">ID of image.</param>
        /// <returns><see cref="IDataReader"/> object.</returns>
        public static IDataReader GetImage(int imageID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Image_GetImage");
            command.AddInParameter("imageID", DbType.Int32, imageID);

            return db.ExecuteReader(command);
        }

        /// <summary>
        /// Get a list of image ids for an image with the specified name
        /// </summary>
        /// <param name="imageName">Name of the image to find.</param>
        /// <param name="onlyInDb">When true, only images with data in database will be returned</param>
        /// <returns></returns>
        public static List<int> FindImage(string imageName, bool onlyInDb)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Image_Find");
            command.AddInParameter("ImageName", DbType.String, imageName);
            command.AddInParameter("ExcludeFiles", DbType.Boolean, onlyInDb);

            var imageIDs = new List<int>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        if (reader["ImageID"] != DBNull.Value)
                        {
                            imageIDs.Add((int)reader["ImageID"]);
                        }
                    }
                }
                catch
                {
                    reader.Close();
                }
            }

            return imageIDs;
        }

        /// <summary>
        /// Validate that a DBValue is of the specified type.
        /// </summary>
        /// <param name="propValue">Value to check.</param>
        /// <param name="propType">Type to validate.</param>
        /// <param name="nullAllowed">Indicates that a DB Null value is ok.</param>
        /// <returns>True if type validated, false otherwise.</returns>
        public static bool ValidateDataIsType(object propValue, Type propType, bool nullAllowed)
        {
            if (propValue == DBNull.Value)
            {
                return nullAllowed;
            }
            
            return propValue.GetType().IsAssignableFrom(propType);
        }

        /// <summary>
        /// Get a value from a table.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="table">Table to select from.</param>
        /// <param name="columnName">Name of column.</param>
        /// <param name="filter">Filter string.</param>
        /// <param name="sort">Sort String.</param>
        /// <param name="defaultValue">Default value to return if value in table is not found or is DBNull.Value</param>
        /// <returns>Value</returns>
        /// <remarks>If more than one row exists that matches the filter, the value will be taken from the first row.</remarks>
        public static T GetValueFromDataTable<T>(DataTable table, string columnName, string filter, string sort, T defaultValue)
        {
            DataRow[] rows = table.Select(filter, sort, DataViewRowState.CurrentRows);

            if (rows.Length > 0)
            {
                return GetValueFromDataRow(rows[0], columnName, defaultValue);
            }
            
            return defaultValue;
        }

        /// <summary>
        /// Get a value from a table row
        /// </summary>
        /// <typeparam name="T">Type of return parameter.</typeparam>
        /// <param name="tableRow">Table row.</param>
        /// <param name="columnName">Column name.</param>
        /// <param name="defaultValue">Default value to return for null value or column does not exist.</param>
        /// <returns>Value</returns>
        public static T GetValueFromDataRow<T>(DataRow tableRow, string columnName, T defaultValue)
        {
            try
            {
                //Check for null
                if (tableRow == null)
                {
                    return defaultValue;
                }

                //check that this column really belongs to the table to avoid exception
                if (tableRow.Table != null && !tableRow.Table.Columns.Contains(columnName))
                    return defaultValue;

                //Try to get value
                return tableRow[columnName] != DBNull.Value ? (T) tableRow[columnName] : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Get a value from a data reader
        /// </summary>
        /// <typeparam name="T">Type of return parameter.</typeparam>
        /// <param name="reader">Data reader</param>
        /// <param name="columnName">Column name.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>Value</returns>
        public static T GetValueFromDataReader<T>(IDataReader reader, string columnName, T defaultValue)
        {
            try
            {
                object cell = reader[columnName];

                if (cell != DBNull.Value)
                {
                    return (T)ChangeType(cell, typeof(T));
                }

                return defaultValue;
            }
            catch 
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// This is helper method, which handles nullable types additionally.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="newType"></param>
        /// <returns></returns>
        private static object ChangeType(object value, Type newType)
        {
            if (newType.IsGenericType && newType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return null;

                NullableConverter converter = new NullableConverter(newType);
                newType = converter.UnderlyingType;
            }

            if (value is IConvertible)
                return Convert.ChangeType(value, newType);
            
            return value;
        }

        /// <summary>
        /// Get a list of values from a specific column.
        /// </summary>
        /// <typeparam name="T">Type of items to return.</typeparam>
        /// <param name="table">Table to select from.</param>
        /// <param name="columnName">Name of the column to return values for.</param>
        /// <param name="filter">Filter string.</param>
        /// <param name="sort">Sort string.</param>
        /// <param name="uniqueOnly">Specify whether to return only unique values.</param>
        /// <returns>List of items from the specific column.</returns>
        public static List<T> ListDataColumnValues<T>(DataTable table, string columnName, string filter, string sort, bool uniqueOnly)
        {
            var list = new List<T>();

            bool noSort = (sort == null || sort.Trim() == string.Empty);
            bool efficientUniqueCheck = false;

            //Simplify if not sorting and not using unique-only
            DataRow[] rows;

            //See if a more efficient unique check can be used
            if (uniqueOnly
                && (noSort
                    || columnName.Equals(sort, StringComparison.InvariantCultureIgnoreCase)))
            {
                rows = table.Select(filter, columnName);
                efficientUniqueCheck = true;
            }
            //Select unsorted for slightly better performance
            else
            {
                rows = noSort ? table.Select(filter) : table.Select(filter, sort);
            }

            //Now loop through the rows and collect the values
            object lastValue = null;

            foreach (DataRow row in rows)
            {
                var columnValue = (T)row[columnName];

                if (efficientUniqueCheck)
                {
                    if (lastValue == null
                        || !ColumnValuesEqual(lastValue, columnValue))
                    {
                        lastValue = columnValue;
                        list.Add(columnValue);
                    }
                }
                else if (!uniqueOnly
                        || !list.Contains(columnValue))
                {
                    list.Add(columnValue);
                }
            }

            return list;
        }


        /// <summary>
        /// Count the number of distinct values for a column in a table.
        /// </summary>
        /// <param name="table">Table to count values in.</param>
        /// <param name="columnName">Column name to count distinct values of.</param>
        /// <param name="filterString">Filter for results.</param>
        /// <param name="includeNull">Include null values in the count.</param>
        /// <param name="uniqueOnly">Include only unique values.</param>
        /// <returns></returns>
        public static int CountValues(DataTable table, string columnName, string filterString, bool includeNull, bool uniqueOnly)
        {
            //Modify the filter string to exclude null values, if no null values should be returned
            if (!includeNull)
            {
                if (filterString == null || filterString.Trim() == string.Empty)
                {
                    filterString = columnName + " IS NOT NULL";
                }
                else
                {
                    filterString = "(" + filterString + ") AND " + columnName + " IS NOT NULL";
                }
            }

            //Select the rows.  If not sorting by unique value, just select and return the count
            if (!uniqueOnly)
            {
                return table.Select(filterString).Length;
            }
            
            //Otherwise, sort by column name and count the unique values
            DataRow[] rows = table.Select(filterString, columnName);

            object lastValue = null;
            int distinctCount = 0;

            //Loop through values
            foreach (DataRow row in rows)
            {
                //Compare value to last value to see if it is distinct.
                if (lastValue == null
                    || !ColumnValuesEqual(lastValue, row[columnName]))
                {
                    distinctCount++;
                    lastValue = row[columnName];
                }
            }

            return distinctCount;
        }


        /// <summary>
        /// Return a boolean indicating if two objects are equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool ColumnValuesEqual(object a, object b)
        {
            // Compares two values to see if they are equal. Also compares DBNULL.Value.
            // Note: If your DataTable contains object fields, then you must extend this
            // function to handle them in a meaningful way if you intend to group on them.

            if (a == DBNull.Value && b == DBNull.Value) //  both are DBNull.Value
            {
                return true;
            }
            if (a == DBNull.Value || b == DBNull.Value) //  only one is DBNull.Value
            {
                return false;
            }

            return (a.Equals(b));  // value type standard comparison
        }





        /// <summary>
        /// Get a a page of data from the input data table
        /// </summary>
        /// <param name="table"></param>
        /// <param name="pageNumber"></param>
        /// <param name="resultsPerPage"></param>
        /// <param name="totalRows"></param>
        /// <returns></returns>
        public static DataTable GetDataPage(DataTable table, int pageNumber, int resultsPerPage, out int totalRows)
        {
            int startIndex = (pageNumber - 1) * resultsPerPage;
            int endIndex = startIndex + resultsPerPage - 1;

            totalRows = 0;

            DataTable outTable = table.Clone();

            DataRow[] inRows = table.Select();

            foreach (DataRow inRow in inRows)
            {
                //Include the row if paging is disabled or item is in desired page
                if ((pageNumber <= 1 || resultsPerPage <= 0) || (totalRows >= startIndex && totalRows <= endIndex))
                {
                    outTable.ImportRow(inRow);
                }

                totalRows++;
            }

            return outTable;
        }

        /// <summary>
        /// Handles the paging of a DataTable in memory.
        /// </summary>
        /// <param name="data">The superset of data.</param>
        /// <param name="pageNumber">The page number to display.</param>
        /// <param name="resultsPerPage">The number of results displayed per page.</param>
        /// <returns>A DataTable which contains the subset of rows needed to display a specific page of data.</returns>
        public static DataTable PageDataTableInMemory(DataTable data, int pageNumber, int resultsPerPage)
        {
            if (data == null) { return null; }
            if (pageNumber == -1 || resultsPerPage == -1) { return data; }

            int rowCount = data.Select().Length;
            DataTable pagedData = data.Clone();

            if (rowCount > 0)
            {
                int firstRow, lastRow;

                if (pageNumber > 1)
                {
                    firstRow = ((pageNumber - 1) * resultsPerPage);
                }
                else
                {
                    firstRow = 0;
                }

                if ((rowCount - firstRow) < resultsPerPage)
                {
                    lastRow = firstRow + (rowCount - firstRow);
                }
                else
                {
                    lastRow = firstRow + resultsPerPage;
                }

                for (int i = firstRow; i < lastRow; i++)
                {
                    pagedData.ImportRow(data.Rows[i]);
                }
            }

            return pagedData;
        }

        ///<summary>
        ///</summary>
        ///<param name="literal"></param>
        ///<param name="ignoreBoundingQuotes"></param>
        ///<returns></returns>
        public static string EscapeValue(string literal, bool ignoreBoundingQuotes)
        {
            //Escape any quotes, if they need it, but don't escape anything that's alread escaped
            if (literal != string.Empty && literal.IndexOf("'") >= 0)
            {
                char[] charArray = literal.ToCharArray();
                var sb = new StringBuilder();

                int minCheck;
                int maxCheck;

                if (ignoreBoundingQuotes)
                {
                    minCheck = 1;
                    maxCheck = charArray.Length - 1;
                }
                else
                {
                    minCheck = 0;
                    maxCheck = charArray.Length;
                }

                //Don't include the opening or closing quotation marks, if directed
                for (int i = 0; i < charArray.Length; i++)
                {
                    sb.Append(charArray[i]);

                    if (i >= minCheck && i < maxCheck)
                    {
                        if (charArray[i] == '\'')
                        {
                            //Check if the value is already escaped.  If this is the last value, we know it's not.
                            if (i < charArray.Length - 1)
                            {
                                if (charArray[i + 1] != '\'')
                                {
                                    sb.Append(charArray[i]);
                                }
                                //Handle case where quote may not be escaped, but whole expression may be bound by quotes
                                else if (i == maxCheck - 1 && ignoreBoundingQuotes && charArray[0] == '\'' && charArray[charArray.Length - 1] == '\'')
                                {
                                    sb.Append(charArray[i]);
                                }
                                //If it is escaped, skip over the second single-quote so we don't try to escape it.
                                else
                                {
                                    i++;
                                    sb.Append(charArray[i]);
                                }
                            }
                            //This is the last value, so add the extra quote
                            else
                            {
                                sb.Append("'");
                            }
                        }
                    }
                }

                literal = sb.ToString();
            }

            return literal;
        }
    }
}

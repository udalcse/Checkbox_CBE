using System;
using System.Data;
using Checkbox.Common;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Forms
{
    /// <summary>
    /// Static class to provide URL Mappings
    /// </summary>
    public static class UrlMapper
    {
        /// <summary>
        /// Get the mapping for the source url
        /// </summary>
        /// <param name="sourceUrl">Requested URL</param>
        /// <returns>URL to rewrite</returns>
        public static string GetMapping(string sourceUrl)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_UrlMappings_GetDestination");
            command.AddInParameter("SourceUrl", DbType.String, sourceUrl);

            string destinationUrl = string.Empty;

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        destinationUrl = DbUtility.GetValueFromDataReader(reader, "DestinationUrl", string.Empty);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            //Return the found mapping
            if (Utilities.IsNotNullOrEmpty(destinationUrl))
            {
                return destinationUrl;
            }

            //Otherwise return the requested url
            return sourceUrl;
        }

        /// <summary>
        /// Get the source url based on the destination url
        /// </summary>
        /// <param name="destinationUrl">Destination url</param>
        /// <returns></returns>
        public static string GetSource(string destinationUrl)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_UrlMappings_GetSource");
            command.AddInParameter("DestinationUrl", DbType.String, destinationUrl);

            string sourceUrl = string.Empty;

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        sourceUrl = DbUtility.GetValueFromDataReader(reader, "SourceUrl", string.Empty);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return sourceUrl;
        }

        /// <summary>
        /// Add/update a mapping from a source URL to a destination URL
        /// </summary>
        /// <param name="sourceUrl">Request URL</param>
        /// <param name="destinationUrl">URL to Rewrite</param>
        public static void AddMapping(string sourceUrl, string destinationUrl)
        {
            SaveMapping(sourceUrl, destinationUrl);
        }

        /// <summary>
        /// Remove a mapping for the specified URL
        /// </summary>
        /// <param name="destinationUrl">URL to remove mapping for</param>
        public static void RemoveMapping(string destinationUrl)
        {
            DeleteMapping(GetSource(destinationUrl));
        }

        /// <summary>
        /// Save the specified mapping to the database.  If a mapping for the source URL exists, it will be updated.
        /// </summary>
        /// <param name="sourceUrl">URL to map from.</param>
        /// <param name="destinationUrl"></param>
        private static void SaveMapping(string sourceUrl, string destinationUrl)
        {
            try
            {
                if (Utilities.IsNotNullOrEmpty(sourceUrl)
                    && Utilities.IsNotNullOrEmpty(destinationUrl))
                {
                    Database db = DatabaseFactory.CreateDatabase();
                    DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_UrlMappings_Upsert");

                    command.AddInParameter("SourceUrl", DbType.String, sourceUrl);
                    command.AddInParameter("DestinationUrl", DbType.String, destinationUrl);

                    db.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessProtected");
                throw;
            }
        }
        
        /// <summary>
        /// Delete a mapping from the database.
        /// </summary>
        /// <param name="sourceUrl">URL to delete mapping for</param>
        private static void DeleteMapping(string sourceUrl)
        {
            try
            {
                if (Utilities.IsNotNullOrEmpty(sourceUrl))
                {
                    Database db = DatabaseFactory.CreateDatabase();
                    DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_UrlMappings_Delete");

                    command.AddInParameter("SourceUrl", DbType.String, sourceUrl);

                    db.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessProtected");
            }
        }

        /// <summary>
        /// Return a boolean if the indicated source mapping exists
        /// </summary>
        /// <param name="sourceUrl">Mapping to check</param>
        /// <returns>Boolean indicating if mapping exists.</returns>
        public static bool SourceMappingExists(string sourceUrl)
        {
            //GetMapping(...) returns the source url in cases when no mapping exists,
            // so use that as the basis for the check.
            return GetMapping(sourceUrl) != sourceUrl;
        }
    }
}

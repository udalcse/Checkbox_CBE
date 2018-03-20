using System;
using System.Data;
using System.Collections.ObjectModel;

using Checkbox.Common;
using Checkbox.Forms.PageLayout.Configuration;

using Prezza.Framework.Data;
using Prezza.Framework.Common;

namespace Checkbox.Forms.PageLayout
{
    /// <summary>
    /// Manager for accessing and getting lists of for ml
    /// </summary>
    public static class PageLayoutTemplateManager
    {
        /// <summary>
        /// Get a list of all layout type names
        /// </summary>
        /// <returns></returns>
        public static ReadOnlyCollection<string> GetTypeNames()
        {
            DataTable typesTable = GetTypeNamesDataTable();

            return new ReadOnlyCollection<string>(DbUtility.ListDataColumnValues<string>(typesTable, "LayoutTemplateTypeName", null, null, true));
        }

        /// <summary>
        /// Create an instance of a page layout temlpate data object of the specified type
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static PageLayoutTemplateData CreatePageLayoutTemplateData(string typeName)
        {
            ArgumentValidation.CheckForEmptyString(typeName, "Page layout type name");

            DataTable typesTable = GetTypeNamesDataTable();

            //Lookup the type
            DataRow[] typeRows = typesTable.Select("LayoutTemplateTypeName = '" + typeName.Replace("'", "''") + "'", null, DataViewRowState.CurrentRows);

            if (typeRows.Length > 0)
            {
                string assemblyName = DbUtility.GetValueFromDataRow(typeRows[0], "TemplateAssembly", string.Empty);
                string className = DbUtility.GetValueFromDataRow(typeRows[0], "TemplateClassName", string.Empty);

                if (Utilities.IsNullOrEmpty(assemblyName) || Utilities.IsNullOrEmpty(className))
                {
                    throw new Exception("Unable to get type information for layout template name: " + typeName);
                }

                PageLayoutTemplateData layoutData = PageLayoutConfigurationFactory.CreatePageLayoutTemplateData(className + "," + assemblyName);

                return layoutData;
            }

            return null;
        }

        /// <summary>
        /// Get the data for an existing layout template.
        /// </summary>
        /// <param name="layoutTemplateID"></param>
        /// <returns></returns>
        public static PageLayoutTemplateData GetPageLayoutTemplateData(int layoutTemplateID)
        {
            string typeName = GetPageLayoutTemplateTypeName(layoutTemplateID);

            if (Utilities.IsNotNullOrEmpty(typeName))
            {
                PageLayoutTemplateData data = CreatePageLayoutTemplateData(typeName);
                data.Load(layoutTemplateID);

                return data;    
            }

            return null;
        }

        /// <summary>
        /// Get the type name for a layout template based on it's id
        /// </summary>
        /// <param name="layoutTemplateID"></param>
        /// <returns></returns>
        public static string GetPageLayoutTemplateTypeName(int layoutTemplateID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_LayoutTemplate_Get");
            command.AddInParameter("LayoutTemplateID", DbType.Int32, layoutTemplateID);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return DbUtility.GetValueFromDataReader<string>(reader, "LayoutTemplateTypeName", null);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// Get a data table containing layout type information
        /// </summary>
        /// <returns></returns>
        private static DataTable GetTypeNamesDataTable()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_LayoutTemplate_ListTypes");

            DataSet ds = db.ExecuteDataSet(command);

            return ds.Tables[0];
        }
    }
}

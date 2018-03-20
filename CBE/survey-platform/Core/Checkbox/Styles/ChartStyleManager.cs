using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Checkbox.Common;
using Checkbox.Forms.Items.UI;
using Checkbox.Pagination;
using Prezza.Framework.Data;
using Prezza.Framework.Security.Principal;

namespace Checkbox.Styles
{
    /// <summary>
    /// Routines and methods for managing chart styles.  Chart styles are an association between a chart appearance and
    /// a "preset" which maps a couple additional properties, such as name, public, etc. to the appearance.
    /// </summary>
    public static class ChartStyleManager
    {
        /// <summary>
        /// List all chart styles available to the specified user.
        /// </summary>
        /// <param name="uniqueIdentifier">Uniqueidentifier to get available styles for.</param>
        /// <param name="onlyEditable">Specify whether to only get styles editable by the user.</param>
        /// <returns>DataSet containing table with list of styles.</returns>
        public static DataSet ListAvailableStyles(string uniqueIdentifier, bool onlyEditable)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AppearancePreset_ListForUser");

            command.AddInParameter("UniqueIdentifier", DbType.String, uniqueIdentifier);
            command.AddInParameter("OnlyEditable", DbType.Boolean, onlyEditable);

            return db.ExecuteDataSet(command);
        }

        /// <summary>
        /// Get paged list of styles available to the user
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="onlyEditable"></param>
        /// <param name="paginationContext"></param>
        /// <returns></returns>
        public static LightweightStyleTemplate[] GetPagedStyleData(ExtendedPrincipal principal, bool onlyEditable, PaginationContext paginationContext)
        {
            //1. Get list from database
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AppearancePreset_ListForUser");

            command.AddInParameter("UniqueIdentifier", DbType.String, principal.Identity.Name);
            command.AddInParameter("OnlyEditable", DbType.Boolean, onlyEditable);

            var templateList = new List<LightweightStyleTemplate>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        int templateId = DbUtility.GetValueFromDataReader(reader, "PresetId", -1);
                        string templateName = DbUtility.GetValueFromDataReader(reader, "PresetName", string.Empty);

                        if (templateId > 0 && Utilities.IsNotNullOrEmpty(templateName))
                        {
                            templateList.Add(new LightweightStyleTemplate
                            {
                                Name = templateName,
                                TemplateId = templateId,
                                IsPublic = DbUtility.GetValueFromDataReader(reader, "Public", false),
                                IsEditable = DbUtility.GetValueFromDataReader(reader, "Editable", false),
                                AppearanceId = DbUtility.GetValueFromDataReader(reader, "AppearanceID", -1),
                                CreatedBy = DbUtility.GetValueFromDataReader(reader, "CreatedBy", string.Empty)//,
                                //DateCreated = DbUtility.GetValueFromDataReader(reader, "DateCreated", DateTime.MinValue)
                            });
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            //Filter -- Support by name and created by
            var filteredList = Utilities.IsNotNullOrEmpty(paginationContext.FilterField)
                ? "CreatedBy".Equals(paginationContext.FilterField, StringComparison.InvariantCultureIgnoreCase)
                        ? templateList.Where(style => style.CreatedBy.Contains(paginationContext.FilterValue))
                        : templateList.Where(style => style.Name.Contains(paginationContext.FilterValue))
                : templateList;

            //Sort
            var sortedList = Utilities.IsNotNullOrEmpty(paginationContext.SortField)
                ? "CreatedBy".Equals(paginationContext.SortField, StringComparison.InvariantCultureIgnoreCase)
                    ? paginationContext.SortAscending
                            ? filteredList.OrderBy(style => style.CreatedBy)
                            : filteredList.OrderByDescending(style => style.CreatedBy)
                    : paginationContext.SortAscending
                            ? filteredList.OrderBy(style => style.Name)
                            : filteredList.OrderByDescending(style => style.Name)
                : filteredList;

            var pagedList = paginationContext.CurrentPage > 0 && paginationContext.PageSize > 0
                ? sortedList.Skip((paginationContext.CurrentPage - 1) * paginationContext.PageSize).Take(paginationContext.PageSize)
                : sortedList;

            paginationContext.ItemCount = templateList.Count;

            return pagedList.ToArray();
        }

        /// <summary>
        /// List all styles available
        /// </summary>
        /// <returns></returns>
        public static DataSet ListAllStyles()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AppearancePreset_ListAll");
            return db.ExecuteDataSet(command);
        }

        /// <summary>
        /// List available chart style ids
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="onlyEditable"></param>
        /// <returns></returns>
        public static List<LightweightStyleTemplate> ListChartStyles(string uniqueIdentifier, bool onlyEditable)
        {
            var data = ListAvailableStyles(uniqueIdentifier, onlyEditable);
            var styleTable = data.Tables[0];

            return (from DataRow styleRow in styleTable.Rows
                    select new LightweightStyleTemplate
                               {
                                   TemplateId = Convert.ToInt32(styleRow["PresetID"]),
                                   AppearanceId = Convert.ToInt32(styleRow["AppearanceID"]),
                                   Name = styleRow["PresetName"].ToString(),
                                   CreatedBy = styleRow["CreatedBy"].ToString(),
                                   IsEditable = Convert.ToBoolean(styleRow["Editable"]),
                                   IsPublic = Convert.ToBoolean(styleRow["Public"])
                               }).ToList();
        }

        /// <summary>
        /// Persist a new style preset to the database.
        /// </summary>
        /// <param name="appearanceID"></param>
        /// <param name="name"></param>
        /// <param name="createdBy"></param>
        /// <param name="isPublic"></param>
        /// <param name="isEditable"></param>
        /// <returns>ID of the created preset.</returns>
        public static int CreateStyle(int appearanceID, string name, string createdBy, bool isPublic, bool isEditable)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AppearancePreset_Insert");

            command.AddInParameter("AppearanceID", DbType.Int32, appearanceID);
            command.AddInParameter("PresetName", DbType.String, name);
            command.AddInParameter("CreatedBy", DbType.String, createdBy);
            command.AddInParameter("Public", DbType.Boolean, isPublic);
            command.AddInParameter("Editable", DbType.Boolean, isEditable);
            //command.AddInParameter("DateCreated", DbType.DateTime, DateTime.Now);
            command.AddOutParameter("PresetID", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            object value = command.GetParameterValue("PresetID");

            if (value == null || value == DBNull.Value)
            {
                throw new Exception("Create Style failed.  A valid preset ID was not returned.");
            }
            
            return (int)value;
        }

        /// <summary>
        /// Update a style preset's properties.  This is done separately from updating the appearance
        /// </summary>
        /// <param name="presetID"></param>
        /// <param name="name"></param>
        /// <param name="isPublic"></param>
        /// <param name="isEditable"></param>
        public static void UpdateStyle(int presetID, string name, bool isPublic, bool isEditable)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AppearancePreset_Update");

            command.AddInParameter("PresetID", DbType.Int32, presetID);
            command.AddInParameter("PresetName", DbType.String, name);
            command.AddInParameter("Public", DbType.Boolean, isPublic);
            command.AddInParameter("Editable", DbType.Boolean, isEditable);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Delete a style preset
        /// </summary>
        /// <param name="presetID"></param>
        public static void DeleteStyle(int presetID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AppearancePreset_Delete");
            command.AddInParameter("PresetID", DbType.Int32, presetID);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>ID of the preset if a preset with the name is found, otherwise</returns>
        public static int? GetStyleIDFromName(string name)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AppearancePreset_FindByName");
            command.AddInParameter("PresetName", DbType.String, name);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        if (reader["PresetID"] != DBNull.Value)
                        {
                            return (int)reader["PresetID"];
                        }
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
        /// Get a simple container with information about a chart style
        /// </summary>
        /// <param name="chartStyleID"></param>
        /// <returns></returns>
        public static LightweightStyleTemplate GetChartStyle(int chartStyleID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AppearancePreset_Get");
            command.AddInParameter("PresetID", DbType.Int32, chartStyleID);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return new LightweightStyleTemplate
                                   {
                                       TemplateId = chartStyleID,
                                       AppearanceId = (int) reader["AppearanceID"],
                                       Name = (string) reader["PresetName"],
                                       CreatedBy = (string) reader["CreatedBy"],
                                       //DateCreated = (DateTime) reader["DateCreated"],
                                       IsEditable = (bool) reader["Editable"],
                                       IsPublic = (bool) reader["Public"]
                                   };
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
        /// Get the appearance data associated with a chart style
        /// </summary>
        /// <param name="chartStyleID">ID of the chart style to get appearance for.</param>
        /// <returns>Chart style appearance</returns>
        public static AppearanceData GetChartStyleAppearance(int chartStyleID)
        {
            var style = GetChartStyle(chartStyleID);

            if (style != null && style.AppearanceId.HasValue)
            {
                return AppearanceDataManager.GetAppearanceData(style.AppearanceId.Value);
            }
            
            return null;
        }

        /// <summary>
        /// Return true if a chart style with the specified name already exists
        /// </summary>
        /// <param name="styleName">Name of the chart style</param>
        /// <returns></returns>
        public static bool IsStyleNameInUse(string styleName)
        {
            int? id = GetStyleIDFromName(styleName);

            return id.HasValue;
        }
    }
}

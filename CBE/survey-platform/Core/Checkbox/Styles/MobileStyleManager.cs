using System;
using System.Collections.Generic;
using System.Data;
using Prezza.Framework.Data;

namespace Checkbox.Styles
{
    /// <summary>
    /// 
    /// </summary>
    public static class MobileStyleManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cssUrl"></param>
        /// <param name="languageCode"> </param>
        /// <returns></returns>
        public static MobileStyle CreateStyle(string name, string cssUrl, string languageCode)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_MobileStyles_Add");
            command.AddInParameter("Name", DbType.String, name);
            command.AddInParameter("CssUrl", DbType.String, cssUrl);
            command.AddInParameter("IsDefault", DbType.Boolean, false);
            command.AddInParameter("LanguageCode", DbType.String, languageCode);
            command.AddOutParameter("StyleID", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            object id = command.GetParameterValue("StyleID");

            if (id == null || id == DBNull.Value)
            {
                throw new Exception("Unable to save template data.");
            }

            int styleId = Convert.ToInt32(id);

            return new MobileStyle
                       {
                           CssUrl = cssUrl,
                           Name = name,
                           StyleId = styleId,
                           IsDefault = false
                       };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="styleId"></param>
        public static void DeleteStyle(int styleId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_MobileStyles_Delete");
            command.AddInParameter("StyleID", DbType.String, styleId);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="languageCode"> </param>
        public static MobileStyle GetDefaultStyle(string languageCode)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_MobileStyles_GetDefault");
            command.AddInParameter("LanguageCode", DbType.String, languageCode);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return new MobileStyle
                        {
                            StyleId = DbUtility.GetValueFromDataReader(reader, "StyleID", -1),
                            CssUrl = DbUtility.GetValueFromDataReader(reader, "CssUrl", string.Empty),
                            Name = DbUtility.GetValueFromDataReader(reader, "Name", string.Empty),
                            IsDefault = DbUtility.GetValueFromDataReader(reader, "IsDefault", false)
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
        /// 
        /// </summary>
        /// <param name="styleId"></param>
        /// <param name="languageCode"> </param>
        public static MobileStyle GetStyle(int styleId, string languageCode)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_MobileStyles_Get");
            command.AddInParameter("StyleID", DbType.String, styleId);
            command.AddInParameter("LanguageCode", DbType.String, languageCode);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                       return new MobileStyle
                       {
                           StyleId = styleId,
                           CssUrl = DbUtility.GetValueFromDataReader(reader, "CssUrl", string.Empty),
                           Name = DbUtility.GetValueFromDataReader(reader, "Name", string.Empty),
                           IsDefault = DbUtility.GetValueFromDataReader(reader, "IsDefault", false)
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
        /// 
        /// </summary>
        /// <param name="languageCode"> </param>
        /// <returns></returns>
        public static IList<MobileStyle> GetAllStyles(string languageCode)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_MobileStyles_GetAll");
            command.AddInParameter("LanguageCode", DbType.String, languageCode);

            List<MobileStyle> styles = new List<MobileStyle>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        styles.Add(new MobileStyle
                        {
                            CssUrl = DbUtility.GetValueFromDataReader(reader, "CssUrl", string.Empty),
                            Name = DbUtility.GetValueFromDataReader(reader, "Name", string.Empty),
                            StyleId = DbUtility.GetValueFromDataReader(reader, "StyleID", -1),
                            IsDefault = DbUtility.GetValueFromDataReader(reader, "IsDefault", false)
                        });
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return styles;
        }
    }
}

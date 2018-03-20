using System.Collections.Generic;
using System.Data;
using Prezza.Framework.Data;

namespace Checkbox.Forms
{
    /// <summary>
    /// SurveySectionManager
    /// </summary>
    public static class SurveySectionManager
    {
        /// <summary>
        /// Gets the section item ids.
        /// </summary>
        /// <param name="sectionId">The section identifier.</param>
        /// <returns></returns>
        public static List<int> GetSectionItemIds(int sectionId)
        {
            var db = DatabaseFactory.CreateDatabase();
            var command = db.GetStoredProcCommandWrapper("ckbx_sp_Sections_GetSectionIds");

            command.AddInParameter("SectionId", DbType.Int32, sectionId);

            List<int> result = new List<int>();

            using (var reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        result.Add(DbUtility.GetValueFromDataReader(reader, "ItemId", 0));
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// Gets all survey section item ids.
        /// </summary>
        /// <param name="surveyId">The survey identifier.</param>
        /// <returns></returns>
        public static List<int> GetAllSurveySectionItemIds(int surveyId)
        {
            var db = DatabaseFactory.CreateDatabase();
            var command = db.GetStoredProcCommandWrapper("ckbx_sp_Sections_GetAllSurveySectionItems");

            command.AddInParameter("SurveyId", DbType.Int32, surveyId);

            List<int> result = new List<int>();

            using (var reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        result.Add(DbUtility.GetValueFromDataReader(reader, "ItemId", 0));
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return result;
        }


        /// <summary>
        /// Deletes the section.
        /// </summary>
        /// <param name="sectionId">The section identifier.</param>
        public static void DeleteSection(int sectionId)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var connection = db.GetConnection())
            {
                try
                {
                    connection.Open();

                    var cmd =
                        db.GetStoredProcCommandWrapper("ckbx_sp_Sections_DeleteSection");

                    cmd.AddInParameter("SectionId", DbType.Int32, sectionId);


                    db.ExecuteNonQuery(cmd);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Adds the section item identifier.
        /// </summary>
        /// <param name="sectionId">The section identifier.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="surveyId">The survey identifier.</param>
        public static void AddSectionItemId(int sectionId, int itemId, int surveyId)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var connection = db.GetConnection())
            {
                try
                {
                    connection.Open();

                    var cmd =
                        db.GetStoredProcCommandWrapper("ckbx_sp_Sections_AddSectionItemId");

                    cmd.AddInParameter("ItemId", DbType.Int32, itemId);
                    cmd.AddInParameter("SectionId", DbType.Int32, sectionId);
                    cmd.AddInParameter("SurveyId", DbType.Int32, surveyId);

                    db.ExecuteNonQuery(cmd);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}

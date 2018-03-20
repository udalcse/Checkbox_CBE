using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Prezza.Framework.Data;
using Checkbox.Analytics;

namespace Checkbox.Forms
{
    /// <summary>
    /// Provides operations which are common for all template managers.
    /// </summary>
    public static class TemplateManager
    {
        /// <summary>
        /// Get template type
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public static TemplateType GetTemplateType(int templateId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_Template_GetType");
            command.AddInParameter("TemplateID", DbType.Int32, templateId);
            command.AddOutParameter("TemplateType", DbType.String, 40);
            db.ExecuteNonQuery(command);
            String templateType = command.GetParameterValue("TemplateType").ToString();

            return (TemplateType)Enum.Parse(typeof(TemplateType), templateType);
        }

        /// <summary>
        /// Get template
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns>Return a template. If a template with this ID doesn't exist, "null" will be returned</returns>
        public static Template GetTemplate(int templateId)
        {
            TemplateType templateType = GetTemplateType(templateId);

            if (templateType == TemplateType.ResponseTemplate)
                return ResponseTemplateManager.GetResponseTemplate(templateId);

            if (templateType == TemplateType.LibraryTemplate)
                return LibraryTemplateManager.GetLibraryTemplate(templateId);

            if (templateType == TemplateType.AnalysisTemplate)
                return AnalysisTemplateManager.GetAnalysisTemplate(templateId);
            
            return null;
        }


    }
}

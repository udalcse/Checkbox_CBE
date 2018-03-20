using System;
using System.Data;
using Checkbox.Analytics.Security;
using Prezza.Framework.Data.Sprocs;
using Prezza.Framework.Security;

namespace Checkbox.Analytics
{
    /// <summary>
    /// Lightweight representation of an analysis template suitable for
    /// authorization, security management purposes and populating lists.
    /// </summary>
    [Serializable]
    [FetchProcedure("ckbx_sp_AnalysisTemplate_GetAccessControllableResource")]
    public class LightweightAnalysisTemplate : LightweightAccessControllable
    {
        /// <summary>
        /// Get/set the date the template was created
        /// </summary>
        [FetchParameter(Name = "DateCreated", DbType = DbType.DateTime, Direction = ParameterDirection.ReturnValue)]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Get/set the date the template was last updated
        /// </summary>
        [FetchParameter(Name = "DateModified", DbType = DbType.DateTime, Direction = ParameterDirection.ReturnValue)]
        public DateTime DateModified { get; set; }

        /// <summary>
        /// Get/set the name of creator
        /// </summary>
        [FetchParameter(Name = "CreatedBy", DbType = DbType.DateTime, Direction = ParameterDirection.ReturnValue)]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Get/set the template id
        /// </summary>
        [FetchParameter(Name = "TemplateID", DbType = DbType.Int32, Direction = ParameterDirection.Input)]
        public override int ID { get; set; }

        /// <summary>
        /// Get/set the template Guid
        /// </summary>
        [FetchParameter(Name = "GUID", DbType = DbType.Guid, Direction = ParameterDirection.ReturnValue)]
        public Guid Guid { get; set; }

        /// <summary>
        /// Get/set the template name
        /// </summary>
        [FetchParameter(Name = "Name", DbType = DbType.String, Direction = ParameterDirection.ReturnValue)]
        public override string Name { get; set; }

        /// <summary>
        /// Get/set text id for title
        /// </summary>
        [FetchParameter(Name = "NameTextID", DbType = DbType.String, Direction = ParameterDirection.ReturnValue)]
        public string NameTextId { get; set; }

        /// <summary>
        /// Get/set style template for report.
        /// </summary>
        [FetchParameter(Name = "StyleTemplateId", DbType = DbType.Int32, Direction = ParameterDirection.ReturnValue, AllowNull=true)]
        public int? StyleTemplateId { get; set; }

        /// <summary>
        /// Get/set response template report is associated with
        /// </summary>
        [FetchParameter(Name = "ResponseTemplateId", DbType = DbType.Int32, Direction = ParameterDirection.ReturnValue)]
        public int? ResponseTemplateId { get; set; }

        /// <summary>
        /// Get/set id of chart style
        /// </summary>
        [FetchParameter(Name = "ChartStyleId", DbType = DbType.Int32, Direction = ParameterDirection.ReturnValue, AllowNull=true)]
        public int? ChartStyleId { get; set; }

        /// <summary>
        /// Get/set start date for date filter
        /// </summary>
        [FetchParameter(Name = "DateFilterStart", DbType = DbType.DateTime, Direction = ParameterDirection.ReturnValue, AllowNull=true)]
        public DateTime? DateFilterStart { get; set; }

        /// <summary>
        /// Get/set end date for date filter
        /// </summary>
        [FetchParameter(Name = "DateFilterEnd", DbType = DbType.DateTime, Direction = ParameterDirection.ReturnValue, AllowNull=true)]
        public DateTime? DateFilterEnd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal LightweightAnalysisTemplate(int id)
        {
            ID = id;
        }

        /// <summary>
        /// Get a security editor for analysis templates
        /// </summary>
        /// <returns></returns>
        public override SecurityEditor GetEditor()
        {
            return new AnalysisSecurityEditor(this);
        }

        /// <summary>
        /// Get list of permission masks supported by analysis templates
        /// </summary>
        public override string[] SupportedPermissionMasks
        {
            get { return new[] {"Analysis.Administer", "Analysis.Edit", "Analysis.Run"}; }
        }

        /// <summary>
        /// Get list of permissions supported by analysis templates
        /// </summary>
        public override string[] SupportedPermissions
        {
            get { return new[] {"Analysis.Administer", "Analysis.Edit", "Analysis.Delete", "Analysis.Run"}; }
        }


        /// <summary>
        /// Create a security policy with the specified permissions
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public override Policy CreatePolicy(string[] permissions)
        {
            return new AnalysisPolicy(permissions);
        }
    }
}
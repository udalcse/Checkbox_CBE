using System;
using System.Data;
using Checkbox.Forms.Security;
using Prezza.Framework.Data.Sprocs;
using Prezza.Framework.Security;

namespace Checkbox.Forms
{
    /// <summary>
    /// Lightweight representation of a response template suitable for
    /// authorization and security management purposes.
    /// </summary>
    [Serializable]
    [FetchProcedure("ckbx_sp_RT_GetAccessControllableResource")]
    public class LightweightResponseTemplate : LightweightAccessControllable
    {
        /// <summary>
        /// Get/set the template Guid
        /// </summary>
        [FetchParameter(Name = "GUID", DbType = DbType.Guid, Direction = ParameterDirection.ReturnValue)]
        public Guid GUID { get; set; }

        /// <summary>
        /// Get/set the template _name
        /// </summary>
        [FetchParameter(Name = "Name", DbType = DbType.String, Direction = ParameterDirection.ReturnValue)]
        public override string Name { get; set; }

        /// <summary>
        /// Get/set the template _id
        /// </summary>
        [FetchParameter(Name = "TemplateID", DbType = DbType.Int32, Direction = ParameterDirection.Input)]
        public override int ID { get; set; }

        /// <summary>
        /// Get entity type of lightweight template
        /// </summary>
        public override string EntityType { get { return "ResponseTemplate"; } }

        /// <summary>
        /// Get/set whether template can be edited while active
        /// </summary>
        [FetchParameter(Name = "AllowSurveyEditWhileActive", DbType = DbType.Boolean, Direction = ParameterDirection.ReturnValue)]
        public bool AllowEditWhileActive { get; set; }

        /// <summary>
        /// Get/set default language for survey
        /// </summary>
        [FetchParameter(Name = "DefaultLanguage", DbType = DbType.String, Direction = ParameterDirection.ReturnValue)]
        public string DefaultLanguage { get; set; }

        /// <summary>
        /// Get/set text id for title
        /// </summary>
        [FetchParameter(Name = "TitleTextId", DbType = DbType.String, Direction = ParameterDirection.ReturnValue)]
        public string TitleTextId { get; set; }

        /// <summary>
        /// Get/set numeric value for security type
        /// </summary>
        [FetchParameter(Name = "SecurityTypeValue", DbType = DbType.Int32, Direction = ParameterDirection.ReturnValue)]
        public int SecurityTypeValue { get; set; }

        /// <summary>
        /// Get/set is active value
        /// </summary>
        [FetchParameter(Name = "IsActive", DbType = DbType.Boolean, Direction = ParameterDirection.ReturnValue)]
        public bool IsActive { get; set; }

        /// <summary>
        /// Get/set activation start date
        /// </summary>
        [FetchParameter(Name = "ActivationStart", DbType = DbType.DateTime, Direction = ParameterDirection.ReturnValue)]
        public DateTime? ActivationStartDate { get; set; }

        /// <summary>
        /// Get/set numeric value for security type
        /// </summary>
        [FetchParameter(Name = "ActivationEnd", DbType = DbType.DateTime, Direction = ParameterDirection.ReturnValue)]
        public DateTime? ActivationEndDate { get; set; }

        ///// <summary>
        ///// Gets or sets a flag indicating whether activation is linked to the periods
        ///// </summary>
        //[FetchParameter(Name = "ActiveDuringCurrentPeriod", DbType = DbType.Boolean, Direction = ParameterDirection.ReturnValue)]
        //public bool ActiveDuringCurrentPeriod { get; set; }

        /// <summary>
        /// Get/set numeric value for security type
        /// </summary>
        [FetchParameter(Name = "MaxTotalResponses", DbType = DbType.Int32, Direction = ParameterDirection.ReturnValue, AllowNull = true)]
        public int? MaxTotalResponses { get; set; }

        /// <summary>
        /// Get/set numeric value for security type
        /// </summary>
        [FetchParameter(Name = "MaxResponsesPerUser", DbType = DbType.Int32, Direction = ParameterDirection.ReturnValue, AllowNull = true)]
        public int? MaxResponsesPerUser { get; set; }

        /// <summary>
        /// Get/set is active value
        /// </summary>
        [FetchParameter(Name = "AllowEdit", DbType = DbType.Boolean, Direction = ParameterDirection.ReturnValue)]
        public bool AllowEdit { get; set; }

        /// <summary>
        /// Get/set is active value
        /// </summary>
        [FetchParameter(Name = "AnonymizeResponses", DbType = DbType.Boolean, Direction = ParameterDirection.ReturnValue)]
        public bool AnonymizeResponses { get; set; }

        /// <summary>
        /// Get/set numeric value for style template id
        /// </summary>
        [FetchParameter(Name = "StyleTemplateId", DbType = DbType.Int32, Direction = ParameterDirection.ReturnValue)]
        public int? StyleTemplateId { get; set; }

        /// <summary>
        /// Get/set text id for title
        /// </summary>
        [FetchParameter(Name = "SupportedLanguagesString", DbType = DbType.String, Direction = ParameterDirection.ReturnValue)]
        public string SupportedLanguagesString { get; set; }

        /// <summary>
        /// Get/set created date
        /// </summary>
        [FetchParameter(Name = "CreatedDate", DbType = DbType.DateTime, Direction = ParameterDirection.ReturnValue)]
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Get/set modified date
        /// </summary>
        [FetchParameter(Name = "ModifiedDate", DbType = DbType.DateTime, Direction = ParameterDirection.ReturnValue)]
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Get/set modified date
        /// </summary>
        [FetchParameter(Name = "CreatedBy", DbType = DbType.String, Direction = ParameterDirection.ReturnValue)]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the SecurityType mask for this <see cref="ResponseTemplate"/>
        /// </summary>
        public SecurityType SecurityType
        {
            get { return (SecurityType)Enum.ToObject(typeof(SecurityType), SecurityTypeValue); }
        }

        /// <summary>
        /// Gets or sets a string array of the languages supported by this <see cref="ResponseTemplate"/>
        /// </summary>
        public string[] SupportedLanguages
        {
            get
            {
                return SupportedLanguagesString.Split(new char[]{';'}, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal LightweightResponseTemplate(int id)
        {
            ID = id;
        }
     
        /// <summary>
        /// Create a security policy with the specified permissions
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public override Policy CreatePolicy(string[] permissions)
        {
            return new FormPolicy(permissions);
        }

        /// <summary>
        /// Get list of permissions masks supported by response templates
        /// </summary>
        public override string[] SupportedPermissionMasks
        {
            get { return new[] { "Form.Administer", "Form.Edit", "Form.Fill", "Analysis.ViewResponses", "Analysis.Analyze" }; }
        }

        /// <summary>
        /// Get a list of permissions supported by response templates
        /// </summary>
        public override string[] SupportedPermissions
        {
            get { return new[] { "Form.Administer", "Form.Create", "Form.Delete", "Form.Edit", "Form.Fill", "Analysis.Create", "Analysis.Responses.View", "Analysis.Responses.Export", "Analysis.Responses.Edit", "Analysis.ManageFilters" }; }
        }

        /// <summary>
        /// Get a security editor for response templates
        /// </summary>
        /// <returns></returns>
        public override SecurityEditor GetEditor()
        {
            return new FormSecurityEditor(this);
        }
    }
}
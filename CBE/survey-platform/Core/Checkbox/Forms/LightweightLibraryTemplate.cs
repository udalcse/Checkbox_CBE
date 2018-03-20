using System;
using System.Data;
using Checkbox.Forms.Security;
using Prezza.Framework.Data.Sprocs;
using Prezza.Framework.Security;
using Checkbox.Globalization.Text;

namespace Checkbox.Forms
{
    /// <summary>
    /// Lightweight representation of a library template suitable for
    /// authorization and security management purposes.
    /// </summary>
    [Serializable]
    [FetchProcedure("ckbx_sp_Library_GetAccessControllableResource")]
    public class LightweightLibraryTemplate : LightweightAccessControllable
    {
        /// <summary>
        /// Get/set the library template id
        /// </summary>
        [FetchParameter(Name = "TemplateID", DbType = DbType.Int32, Direction = ParameterDirection.Input)]
        public override int ID { get; set; }

        /// <summary>
        /// Get/set the name of the library template
        /// </summary>
        public override string Name
        {
            get { return TextManager.GetText(NameTextID, TextManager.DefaultLanguage); }
            set { base.Name = value; }
        }
        /// <summary>
        /// Get the description of the library template
        /// </summary>
        public string Description
        {
            get { return TextManager.GetText(DescriptionTextID, TextManager.DefaultLanguage); }
        }

        /// <summary>
        /// Get/set the library template name textId
        /// </summary>
        [FetchParameter(Name = "NameTextID", DbType = DbType.String, Direction = ParameterDirection.ReturnValue)]
        public string NameTextID { get; set; }

        /// <summary>
        /// Get/set the library template description textId
        /// </summary>
        [FetchParameter(Name = "DescriptionTextID", DbType = DbType.String, Direction = ParameterDirection.ReturnValue)]
        public string DescriptionTextID { get; set; }

        /// <summary>
        /// Get/set the library template menu availability
        /// </summary>
        [FetchParameter(Name = "ShowInMenu", DbType = DbType.Boolean, Direction = ParameterDirection.ReturnValue)]
        public bool? ShowInMenu { get; set; }

        /// <summary>
        /// Get entity type of lightweight template
        /// </summary>
        public override string EntityType { get { return "ItemLibrary"; } }

        ///<summary>
        ///</summary>
        [FetchParameter(Name = "CreatedBy", DbType = DbType.String, Direction = ParameterDirection.ReturnValue)]
        public string CreatedBy { get; set; }

        ///<summary>
        ///</summary>
        [FetchParameter(Name = "ModifiedDate", DbType = DbType.DateTime, Direction = ParameterDirection.ReturnValue)]
        public DateTime? ModifiedDate { get; set; }

        ///<summary>
        ///</summary>
        [FetchParameter(Name = "CreatedDate", DbType = DbType.DateTime, Direction = ParameterDirection.ReturnValue)]
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Get list of permissions masks supported by response templates
        /// </summary>
        public override string[] SupportedPermissionMasks
        {
            get { return new[] { "Library.Edit", "Library.View" }; }
        }

        /// <summary>
        /// Get a list of permissions supported by response templates
        /// </summary>
        public override string[] SupportedPermissions
        {
            get { return new[] { "Library.Create", "Library.Delete", "Library.Edit", "Library.View" }; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override SecurityEditor GetEditor()
        {
            return new LibrarySecurityEditor(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public override Policy CreatePolicy(string[] permissions)
        {
            return new LibraryPolicy(permissions);
        }
    }
}

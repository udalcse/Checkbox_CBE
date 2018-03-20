using System;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Page;
using Checkbox.Web;
using Checkbox.Web.UI.Controls.Security;

namespace CheckboxWeb.Security
{
    public partial class GrantAccessDialog : SecurityEditorPage
    {
        /// <summary>
        /// Get boolean indicating if parent should be refreshed on close.
        /// </summary>
        [QueryParameter("r", DefaultValue="true")]
        public bool RefreshParentOnClose { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected override SecurityEditorControl GrantAccessControl { get { return _grantAccess; } }

        /// <summary>
        /// 
        /// </summary>
        protected override SecuredResourceType SecuredResourceType
        {
            get { return ContextData.SecuredResourceType; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override int SecuredResourceId
        {
            get { return ContextData.SecuredResourceId; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string SecuredResourceName
        {
            get { return ContextData.SecuredResourceName; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        protected override int SecuredResourceDefaultPolicyId
        {
            get { return ContextData.SecuredResourceDefaultPolicyId; }
        }

        /// <summary>
        /// Page init
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Page.Title = WebTextManager.GetText("/pageText/security/grantAccessDialog.aspx/title");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CloseButton_Click(object sender, EventArgs e)
        {
            string script = RefreshParentOnClose
                 ? "closeWindowAndRefreshParentPage();"
                 : "closeWindow();";

            Page.ClientScript.RegisterStartupScript(GetType(), "CloseWindow", script, true);
        }
    }
}

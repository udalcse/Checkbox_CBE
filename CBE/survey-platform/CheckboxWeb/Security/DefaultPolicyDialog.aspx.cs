using System;
using System.Web.UI;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Page;
using Checkbox.Web;
using Checkbox.Web.UI.Controls.Security;

namespace CheckboxWeb.Security
{
    public partial class DefaultPolicyDialog : SecurityEditorPage
    {
        /// <summary>
        /// Get boolean indicating if parent should be refreshed on close.
        /// </summary>
        [QueryParameter("r")]
        public bool RefreshParentOnClose { get; set; }


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
        /// 
        /// </summary>
        protected override SecurityEditorControl DefaultPolicyEditorControl { get { return _policyEditor; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            _policyEditor.DefaultPolicyId = ContextData.SecuredResourceDefaultPolicyId;
            Page.Title = WebTextManager.GetText("/pageText/security/defaultPolicyDialog.aspx/title");
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();
            RegisterClientScriptInclude("HideStatus", ResolveUrl("~/Resources/HideStatus.js"));
            ClientScript.RegisterStartupScript(GetType(), "HideStatus", "HideStatus();", true);


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DefaultPolicyEditor_Closed(object sender, EventArgs e)
        {
            string script = RefreshParentOnClose
                ? "closeWindowAndRefreshParentPage();"
                : "closeWindow();";

            Page.ClientScript.RegisterStartupScript(GetType(), "CloseWindow", script, true);
        }
    }
}

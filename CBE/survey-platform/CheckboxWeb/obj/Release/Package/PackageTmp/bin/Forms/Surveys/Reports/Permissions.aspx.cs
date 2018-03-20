using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Security;
using Checkbox.Forms;
using Checkbox.Security;
using Checkbox.Analytics;
using Checkbox.Users;


namespace CheckboxWeb.Forms.Surveys.Reports
{
    public partial class Permissions : SecurityEditorPage
    {
        private AnalysisTemplate _analysisTemplate;

        [QueryParameter("r")]
        public int? ReportId { get; set; }

        /// <summary>
        /// Get boolean indicating this page redirects to shared security editor page.
        /// </summary>
        protected override bool IsRedirect 
        { 
            get 
            { 
                return !Checkbox.Management.ApplicationManager.UseSimpleSecurity; 
            } 
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string PageRequiredRolePermission
        {
            get { return "Analysis.Create"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public AnalysisTemplate Report
        {
            get
            {
                if (_analysisTemplate == null && ReportId.HasValue && ReportId.Value > 0)
                {
                    _analysisTemplate = AnalysisTemplateManager.GetAnalysisTemplate(ReportId.Value);
                    _analysisTemplate.Name = WebTextManager.GetText(_analysisTemplate.NameTextID);
                }

                return _analysisTemplate;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override SecuredResourceType SecuredResourceType
        {
            get { return SecuredResourceType.Report; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override int SecuredResourceId
        {
            get { return Report.ID.Value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string SecuredResourceName
        {
            get { return Report.Name; }
        }

        /// <summary>
        /// Get controllable entity for security checks
        /// </summary>
        /// <returns></returns>
        protected override IAccessControllable GetControllableEntity()
        {
            return Report;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override int SecuredResourceDefaultPolicyId
        {
            get { return Report.DefaultPolicyID.Value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string ControllableEntityRequiredPermission { get { return "Analysis.Administer"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            try
            {

                base.OnPageInit();
                if (!Checkbox.Management.ApplicationManager.UseSimpleSecurity)
                {
                    Response.Redirect(ResolveUrl("~/Security/SecurityEditor.aspx"), false);
                }
                else
                {
                    StoreSecurityEditorData();

                    //
                    if (Report.DefaultPolicy.Permissions.Contains("Analysis.Run"))
                        _publicReport.Checked = true;
                    else
                        _privateReport.Checked = true;

                    //subscribe event
                    Master.OkClick += new System.EventHandler(Master_OkClick);
                    //Load tab script
                    RegisterClientScriptInclude(
                        "jquery.ckbxtab.js",
                        ResolveUrl("~/Resources/jquery.ckbxtab.js"));
                }
            }
            catch (System.Exception ex)
            {
                Master.ShowError(ex.Message, ex);
            }

        }

        void Master_OkClick(object sender, System.EventArgs e)
        {
            try
            {

                //save public or private values
                if (_privateReport.Checked)
                {
                    AccessManager.RemovePermissionsFromDefaultPolicy(Report, UserManager.GetCurrentPrincipal(), "Analysis.Run");
                }
                else
                {
                    AccessManager.AddPermissionsToDefaultPolicy(Report, UserManager.GetCurrentPrincipal(), "Analysis.Run");
                }
                Master.CloseDialog(null);
            }
            catch (System.Exception ex)
            {
                Master.ShowError(ex.Message, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GetRequiredPermissionForAclEdit()
        {
            return ControllableEntityRequiredPermission;
        }

    }
}

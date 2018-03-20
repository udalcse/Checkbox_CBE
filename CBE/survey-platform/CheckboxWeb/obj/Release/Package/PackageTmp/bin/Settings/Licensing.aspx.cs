using Checkbox.Management;
using Checkbox.Management.Licensing.Limits;
using Checkbox.Web;
using Checkbox.Web.Page;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Settings
{
    public partial class Licensing : SettingsPage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void  OnPreLoad(System.EventArgs e)
        {
            base.OnPreLoad(e);

            if (Page.IsPostBack) return;

            var yes = WebTextManager.GetText("/common/yes");
            var no = WebTextManager.GetText("/common/no");

            _serialNumber.Text = ActiveLicense.LicenseId;
            _licenseType.Text = (ActiveLicense.LicenseKey == "Checkbox5" ? "Checkbox 6" : ActiveLicense.LicenseKey) + (ActiveLicense.IsTrial ? " (trial)" : "");
            _editorLimit.Text = GetSurveyEditorLimit();
            _allowInvitations.Text = ApplicationManager.AppSettings.AllowInvitations ? yes : no;
            _allowMultilanguageSupport.Text = ApplicationManager.AppSettings.AllowMultiLanguage ? yes : no;
            _allowNativeSpssExport.Text = ApplicationManager.AppSettings.AllowNativeSpssExport ? yes : no;

            if (ActiveLicense.SurveyEditorLimit == null) return;
            _users.DataSource = ActiveLicense.SurveyEditorLimit.UsersInRolePermissionLimit;
            _users.DataBind();
        }

        /// <summary>
        /// Get the number of allowed survey editors
        /// </summary>
        private string GetSurveyEditorLimit()
        {
			var limit = ActiveLicense.SurveyEditorLimit;
            //limit.Initialize(ActiveLicense);

            long? count = null; // limit.RuntimeLimitValue;

			if (limit != null)
				count = limit.RuntimeLimitValue;

            return count.HasValue ? count.Value.ToString() : WebTextManager.GetText("/pageText/settings/licensing.aspx/noLimit");
        }

        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            //Set up the page title with link back to mananger
            var titleControl = new PlaceHolder();

            var managerLink = new HyperLink
                                  {
                                      NavigateUrl = "~/Settings/Manage.aspx",
                                      Text = WebTextManager.GetText("/pageText/settings/manage.aspx/title")
                                  };

            var pageTitleLabel = new Label {Text = " - "};
            pageTitleLabel.Text += WebTextManager.GetText("/pageText/settings/licensing.aspx/title");

            titleControl.Controls.Add(managerLink);
            titleControl.Controls.Add(pageTitleLabel);

            Master.SetTitleControl(titleControl);
            Master.HideDialogButtons();
        }
    }
}

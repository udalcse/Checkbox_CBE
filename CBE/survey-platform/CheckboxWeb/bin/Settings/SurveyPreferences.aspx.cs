using System;
using System.Web.UI.WebControls;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Styles;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Web.UI.Controls;

namespace CheckboxWeb.Settings
{
    public partial class SurveyPreferences : SettingsPage
    {
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            //
            if (!Page.IsPostBack)
            {

                //Set up the page title with link back to mananger
                PlaceHolder titleControl = new PlaceHolder();

                HyperLink managerLink = new HyperLink();
                managerLink.NavigateUrl = "~/Settings/Manage.aspx";
                managerLink.Text = WebTextManager.GetText("/pageText/settings/manage.aspx/title");

                Label pageTitleLabel = new Label();
                pageTitleLabel.Text = " - ";
                pageTitleLabel.Text += WebTextManager.GetText("/pageText/settings/surveyPreferences.aspx/title");

                titleControl.Controls.Add(managerLink);
                titleControl.Controls.Add(pageTitleLabel);

                Master.SetTitleControl(titleControl);


                _defaultSurveySecurity.SelectedValue = ApplicationManager.AppSettings.DefaultSurveySecurityType.ToString();
                _defaultOptionEntryType.SelectedValue = ApplicationManager.AppSettings.DefaultOptionEntryType.ToString();
                _defaultSurveyQuestionEditorType.SelectedIndex = ApplicationManager.AppSettings.DefaultQuestionEditorView.Equals("html", StringComparison.InvariantCultureIgnoreCase) ? 0 : 1;

                _confirmWhenDeletingSurveyItemsAndPages.Checked = ApplicationManager.AppSettings.ShowDeleteConfirmationPopups;
                _enableCustomSurveyUrls.Checked = ApplicationManager.AppSettings.AllowSurveyUrlRewriting;
                _allowEditingOfActiveSurveys.Checked = ApplicationManager.AppSettings.AllowEditActiveSurvey;
                _saveWhenNavigatingBack.Checked = ApplicationManager.AppSettings.SavePartialResponsesOnBackNavigation;
                _setSurveyDefaultButton.Checked = ApplicationManager.AppSettings.SetSurveyDefaultButton;
                _allowEditSurveyStyleTemplate.Checked = ApplicationManager.AppSettings.AllowEditSurveyStyleTemplate;
                _displayHtmlItemAsPlainText.Checked = ApplicationManager.AppSettings.DisplayHtmlItemsAsPlainText;
                _includeIncompleteResponsesToTotalAmount.Checked = ApplicationManager.AppSettings.IncludeIncompleteResponsesToTotalAmount;
            }


            BindStyleDropDown(StyleTemplateType.PC, _defaultSurveyStyle, _noPublicStyles, ApplicationManager.AppSettings.DefaultStyleTemplate.ToString());
            BindStyleDropDown(StyleTemplateType.Tablet, _defaultSurveyStyleTablet, _noPublicStylesTablet, ApplicationManager.AppSettings.DefaultStyleTemplateTablet.ToString());
            BindStyleDropDown(StyleTemplateType.SmartPhone, _defaultSurveyStyleSmartPhone, _noPublicStylesSmartPhone, ApplicationManager.AppSettings.DefaultStyleTemplateSmartPhone.ToString());
        }

        private void BindStyleDropDown(StyleTemplateType type, DropDownList ddl, MultiLanguageLabel lbl, string value)
        {
            //Dynamic lists must always be rebound, regardless of postback
            ddl.Items.Add(new ListItem(WebTextManager.GetText("/pageText/settings/surveyPreferences.aspx/noneSelected"), "-1"));
            foreach (LightweightStyleTemplate style in StyleTemplateManager.ListStyleTemplates(UserManager.GetCurrentPrincipal(), type))
            {
                if (style.IsPublic)
                {
                    ddl.Items.Add(new ListItem(style.Name, style.TemplateId.ToString()));
                }
            }

            //The list alwayse contains a "none" option
            if (ddl.Items.Count == 1)
            {
                ddl.Visible = false;
                lbl.Visible = true;
            }

            if (value != null && value != "-1")
            {
                ddl.SelectedValue = value;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            Master.OkClick += new EventHandler(Master_OkClick); 
        }

        void Master_OkClick(object sender, EventArgs e)
        {
            ApplicationManager.AppSettings.DefaultSurveySecurityType = (SecurityType)Enum.Parse( typeof(SecurityType), _defaultSurveySecurity.SelectedValue);
            //ApplicationManager.AppSettings.DefaultEditView = (AppSettings.EditSurveyView)Enum.Parse(typeof(AppSettings.EditSurveyView), _defaultFormEditorView.SelectedValue);
            ApplicationManager.AppSettings.DefaultOptionEntryType = (AppSettings.OptionEntryType)Enum.Parse(typeof(AppSettings.OptionEntryType), _defaultOptionEntryType.SelectedValue);
            ApplicationManager.AppSettings.DefaultQuestionEditorView = _defaultSurveyQuestionEditorType.SelectedValue;

            ApplicationManager.AppSettings.DefaultStyleTemplate = Convert.ToInt32(_defaultSurveyStyle.SelectedValue);
            ApplicationManager.AppSettings.DefaultStyleTemplateSmartPhone = Convert.ToInt32(_defaultSurveyStyleSmartPhone.SelectedValue);
            ApplicationManager.AppSettings.DefaultStyleTemplateTablet = Convert.ToInt32(_defaultSurveyStyleTablet.SelectedValue);

            ApplicationManager.AppSettings.ShowDeleteConfirmationPopups = _confirmWhenDeletingSurveyItemsAndPages.Checked;
            ApplicationManager.AppSettings.AllowSurveyUrlRewriting = _enableCustomSurveyUrls.Checked;
            ApplicationManager.AppSettings.AllowEditActiveSurvey = _allowEditingOfActiveSurveys.Checked;
            ApplicationManager.AppSettings.SavePartialResponsesOnBackNavigation = _saveWhenNavigatingBack.Checked;

            ApplicationManager.AppSettings.SetSurveyDefaultButton = _setSurveyDefaultButton.Checked;
            ApplicationManager.AppSettings.AllowEditSurveyStyleTemplate = _allowEditSurveyStyleTemplate.Checked;
            ApplicationManager.AppSettings.DisplayHtmlItemsAsPlainText = _displayHtmlItemAsPlainText.Checked;
            ApplicationManager.AppSettings.IncludeIncompleteResponsesToTotalAmount = _includeIncompleteResponsesToTotalAmount.Checked;

            Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/updateSuccessful"), StatusMessageType.Success);
        }
    }
}

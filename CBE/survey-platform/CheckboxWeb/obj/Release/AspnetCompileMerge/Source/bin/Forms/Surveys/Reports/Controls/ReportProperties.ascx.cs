using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Styles;
using Checkbox.Users;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls
{
    public partial class ReportProperties : Checkbox.Web.Common.UserControlBase
    {
        private int _surveyId = -1;
        public int SurveyId
        {
            get { return _surveyId; }
            set { _surveyId = value; }
        }

        public string SurveyName
        {
            get; set;
        }

        public string ReportName
        {
            get
            {
                return ApplicationManager.AppSettings.AllowHTMLNames ? _reportName.Text.Trim() : Server.HtmlEncode(_reportName.Text.Trim());
            }
            set { _reportName.Text = value; }
        }

        //Determine if it's a new report or not. 
        public bool IsNewReport
        {
            get { return _reportTypePlace.Visible; }
            set { _reportTypePlace.Visible = value; }
        }


        public int? StyleTemplateId
        {
            get
            {
                int id;
                if (int.TryParse(_templates.SelectedValue, out id))
                {
                    return id;
                }

                return null;
            }
            set
            {
                if (value.HasValue && _templates.Items.FindByValue(value.Value.ToString())!=null)
                {
                    _templates.SelectedValue = value.Value.ToString();
                }
            }
        }

        public string StyleTemplateName
        {
            get
            {
                if (Utilities.IsNotNullOrEmpty(_templates.SelectedItem.Text))
                {
                    return _templates.SelectedItem.Text;
                }

                return WebTextManager.GetText("/pageText/reportProperties.ascx/noneSelected");
            }
        }

        public int ChartStyleId
        {
            get
            {
                int id;
                if (int.TryParse(_chartStyles.SelectedValue, out id))
                {
                    return id;
                }

                return -1;
            }
            set
            {
                if ( _chartStyles.Items.FindByValue(value.ToString()) != null)
                    _chartStyles.SelectedValue = value.ToString();
            }
        }

        public bool DisplaySurveyTitle
        {
            get
            {
                return _displaySurveyTitle.Checked;
            }
            set
            {
                _displaySurveyTitle.Checked = value;
            }
        }

        public bool DisplayPdfExportButton
        {
            get
            {
                return _displayPdfExportButton.Checked;
            }
            set
            {
                _displayPdfExportButton.Checked = value;
            }
        }

        public string ChartStyleName
        {
            get
            {
                if (Utilities.IsNotNullOrEmpty(_chartStyles.SelectedItem.Text))
                {
                    return _chartStyles.SelectedItem.Text;
                }

                return WebTextManager.GetText("/pageText/reportProperties.ascx/noneSelected");
            }
        }


        public bool IsWizard
        {
            get { return _reportTypeWizardRad.Checked; }
        }

        public Label NameValidator
        {
            get { return _nameInUseValidator; }
        }

        /// <summary>
        /// Handle page initialization
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LoadTemplates();
            LoadChartStyles();

            //Check > 1 because the LoadTemplates() adds a none selected option
            if (_templates.Items.Count > 1)
            {
                _noTemplates.Visible = false;
                _templates.Visible = true;
            }
            else
            {
                _templates.Visible = false;
                _noTemplates.Visible = true;
            }

            //Check > 1 because the LoadChartStyels() adds a none selected option
            if (_chartStyles.Items.Count > 1)
            {
                _noChartStyles.Visible = false;
                _chartStyles.Visible = true;
            }
            else
            {
                _chartStyles.Visible = false;
                _noChartStyles.Visible = true;
            }
            if (ApplicationManager.UseSimpleSecurity)
            {
                _simpleSecurityOptions.Visible = true;
                PopulateSecurityOptions();
            }
            else
            {
                _simpleSecurityOptions.Visible = false;
            }
        }

        private void PopulateSecurityOptions()
        {
            _securityOptions.Items.FindByValue("PUBLIC").Text = WebTextManager.GetText("/pageText/reportProperties.ascx/public");
            _securityOptions.Items.FindByValue("PRIVATE").Text = WebTextManager.GetText("/pageText/reportProperties.ascx/private");
            _securityOptions.Items.FindByValue("REGISTERED").Text = WebTextManager.GetText("/pageText/reportProperties.ascx/registered");
        }

        /// <summary>
        /// Load
        /// </summary>
        protected override void OnLoad(System.EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (_chartStyles.Items.FindByValue(ApplicationManager.AppSettings.DefaultChartStyle.ToString()) != null)
                    _chartStyles.SelectedValue = ApplicationManager.AppSettings.DefaultChartStyle.ToString();
                if (_templates.Items.FindByValue(ApplicationManager.AppSettings.DefaultStyleTemplate.ToString()) != null)
                    _templates.SelectedValue = ApplicationManager.AppSettings.DefaultStyleTemplate.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadTemplates()
        {
            _templates.Items.Clear();
            _templates.DataTextField = "Value";
            _templates.DataValueField = "Key";
            _templates.DataSource = StyleTemplateManager.ListStyleTemplatesForDataBinding(UserManager.GetCurrentPrincipal());
            _templates.DataBind();

            ListItem defaultItem = new ListItem(WebTextManager.GetText("/pageText/reportProperties.ascx/noneSelected"), "NONE_SELECTED");
            _templates.Items.Insert(0, defaultItem);
        }

        /// <summary>
        /// Load chart styles
        /// </summary>
        private void LoadChartStyles()
        {
            _chartStyles.Items.Clear();
            _chartStyles.DataTextField = "PresetName";
            _chartStyles.DataValueField = "PresetID";

            if (UserManager.GetCurrentPrincipal() == null)
            {
                _chartStyles.DataSource = ChartStyleManager.ListAvailableStyles(string.Empty, false);
            }
            else if (UserManager.GetCurrentPrincipal().IsInRole("System Administrator"))
            {
                _chartStyles.DataSource = ChartStyleManager.ListAllStyles();
            }
            else
            {
                _chartStyles.DataSource = ChartStyleManager.ListAvailableStyles(UserManager.GetCurrentPrincipal().Identity.Name, false);
            }

            _chartStyles.DataBind();

            ListItem defaultItem = new ListItem(WebTextManager.GetText("/pageText/reportProperties.ascx/noneSelected"), "NONE_SELECTED");
            _chartStyles.Items.Insert(0, defaultItem);
        }

//        /// <summary>
//        /// Set security policy for simple security
//        /// </summary>
//        private void SetSimpleSecurityPolicy()
//        {
//            SecurityEditor editor = CurrentAnalysisTemplate.GetEditor();
//            editor.Initialize(UserManager.GetCurrentPrincipal());
//
//            switch (_securityOptions.SelectedValue)
//            {
//                case "PUBLIC":
//                    editor.SetDefaultPolicy(new Policy(new string[] { "Analysis.Run", "Analysis.Edit" }));
//                    editor.RemoveAccess(GroupManager.GetEveryoneGroup());
//                    editor.SaveAcl();
//                    break;
//                
//                case "PRIVATE":
//                    editor.SetDefaultPolicy(new Policy(new string[] { }));
//                    editor.RemoveAccess(GroupManager.GetEveryoneGroup());
//                    editor.SaveAcl();
//                    break;
//
//                case "REGISTERED":
//                    editor.SetDefaultPolicy(new Policy(new string[] { }));
//                    editor.ReplaceAccess(GroupManager.GetEveryoneGroup(), new string[] { "Analysis.Run", "Analysis.Edit" });
//                    editor.SaveAcl();
//                    break;
//                default:
//                    break;
//            }
//        }
    }
}

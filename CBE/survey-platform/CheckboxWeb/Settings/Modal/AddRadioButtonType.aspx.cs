using Checkbox.Web.Page;
using System;
using System.Linq;
using System.Web.UI.WebControls;
using Checkbox.Web;
using Checkbox.Users;
using Checkbox.Security;
using CheckboxWeb.Services;
using Checkbox.Pagination;
using Checkbox.Web.Providers;

namespace CheckboxWeb.Settings.Modal
{
    public partial class AddRadioButtonType : SecuredPage
    {
        #region ----- Private fields ----- 

        private const string ViewStateFieldNameKey = "fieldName";
     
        private const string ViewStateRadioButtonKey = "RadioButtonField";

        private RadioButtonField RadioButtonField
        {
            get
            {
                var state = ViewState[ViewStateRadioButtonKey];
                return state as RadioButtonField;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RadioButtonField radioButtonField = new RadioButtonField
                {
                    Name = ViewState[ViewStateFieldNameKey]?.ToString() ?? Request[ViewStateFieldNameKey]
                };

                ViewState[ViewStateRadioButtonKey] = radioButtonField;
            }
        }

        protected override void OnPageInit()
        {
            base.OnPageInit();

            //Set page title
            Master.Title = WebTextManager.GetText("/pageText/settings/modal/addRadioButtonType");
            Master.OkClick += SaveButtonClickEventHandler;

            _addRowBtn.Click += _addRowBtn_Click;

            //Helper for uframe
            RegisterClientScriptInclude(
                "htmlparser.js",
                ResolveUrl("~/Resources/htmlparser.js"));

            //Helper for loading pages into divs
            RegisterClientScriptInclude(
                "UFrame.js",
                ResolveUrl("~/Resources/UFrame.js"));
        }

        #region ----- Event handlers -----

        protected void _addRowBtn_Click(object sender, EventArgs e)
        {
            if (this.RadioButtonField.HasOption(_newRadioBtnOption.Text))
            {
                Master.ShowStatusMessage("Choices must be unique", StatusMessageType.Error);
                return;
            }

            var currentState = this.RadioButtonField;
            currentState.AddOption(_newRadioBtnOption.Text, _newRadioBtn.Checked);
            SaveRadioBtnState(currentState);

            _optionsRepeater.DataSource = RadioButtonField.Options;
            _optionsRepeater.DataBind();

            ClearInputValues();
            Master.HideStatusMessage();
        }

        protected void _removeOption_Click(object sender, EventArgs e)
        {
            var btn = sender as LinkButton;
            if (btn != null)
            {
                var index = int.Parse(btn.Attributes["data-index"]);

                var currentState = this.RadioButtonField;
                currentState.Options.RemoveAt(index);
                SaveRadioBtnState(currentState);

                _optionsRepeater.DataSource = RadioButtonField.Options;
                _optionsRepeater.DataBind();
            }
        }

        protected void _newRadioBtn_CheckChanged(object sender, EventArgs e)
        {
            var btn = sender as RadioButton;
            if (btn != null)
            {
                if (btn.Checked)
                {
                    var currentState = this.RadioButtonField;
                    currentState.UnCheckOptions();
                    SaveRadioBtnState(currentState);
                }

                _optionsRepeater.DataSource = RadioButtonField.Options;
                _optionsRepeater.DataBind();
            }
        }

        protected void _radioFieldOptionSelect_CheckChanged(object sender, EventArgs e)
        {
            var btn = sender as RadioButton;
            if (btn != null)
            {
                var index = int.Parse(btn.Attributes["data-index"]);

                if (btn.Checked)
                {
                    var currentState = this.RadioButtonField;
                    currentState.CheckOption(index);
                    SaveRadioBtnState(currentState);

                    _newRadioBtn.Checked = false;
                }

                _optionsRepeater.DataSource = RadioButtonField.Options;
                _optionsRepeater.DataBind();
            }
        }

        private void SaveButtonClickEventHandler(object sender, EventArgs e)
        {
            if (Page.IsValid && ValidateSelectedOptions())
            {
                if (!string.IsNullOrWhiteSpace(_newRadioBtnOption.Text))
                {
                    var currentState = this.RadioButtonField;
                    currentState.AddOption(_newRadioBtnOption.Text, _newRadioBtn.Checked);
                    SaveRadioBtnState(currentState);
                }

                if (_applyToAllCkbx.Checked && RadioButtonField.Options.Any(o => o.IsSelected))
                {
                    ProfileManager.AddRadioButtonField(this.RadioButtonField, null);
                }
                else {
                    ProfileManager.AddRadioButtonField(this.RadioButtonField,
                    AuthenticationService.GetCurrentPrincipal(string.Empty).UserGuid);
                }

                Master.CloseDialog(null);
            }
        }

        #endregion

        #region ----- Helpers  -----

        private void SaveRadioBtnState(RadioButtonField radioButtonField)
        {
            ViewState[ViewStateRadioButtonKey] = radioButtonField;
        }

        private void ClearInputValues()
        {
            _newRadioBtnOption.Text = string.Empty;
            _newRadioBtn.Checked = false;
        }

        /// <summary>
        /// Validates the selected options.
        /// </summary>
        /// <returns></returns>
        private bool ValidateSelectedOptions()
        {
            bool isValid = true;

            if (!this.RadioButtonField.Options.Any() && string.IsNullOrWhiteSpace(_newRadioBtnOption.Text))
            {
                Master.ShowStatusMessage("Please provide at least one option.", StatusMessageType.Error);
                isValid = false;
            }
            else if (this.RadioButtonField.HasOption(_newRadioBtnOption.Text))
            {
                Master.ShowStatusMessage("Choices must be unique", StatusMessageType.Error);
                isValid = false; 
            }
            else if (string.IsNullOrWhiteSpace(_newRadioBtnOption.Text) && _newRadioBtn.Checked)
            {
                Master.ShowStatusMessage("Please provide title for selected option", StatusMessageType.Error);
                isValid = false; 
            }

            if (isValid)
            {
                Master.HideStatusMessage();
            }

            return isValid;
        }

        #endregion

    }
}
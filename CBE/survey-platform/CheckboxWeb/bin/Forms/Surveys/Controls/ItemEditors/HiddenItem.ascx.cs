using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Web;
using Checkbox.Management;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Base user control for editing hidden items
    /// </summary>
    public partial class HiddenItem : UserControlItemEditorBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected override Control PreviewContainer
        {
            get { return _previewPlace; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Load tab script
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));

            //If this is checkboxOnline - remove Session/Cookie variables.
            if (ApplicationManager.AppSettings.EnableMultiDatabase)
            {                
                _itemTypeList.Items.Remove(_itemTypeList.Items.FindByValue("Session"));
                _itemTypeList.Items.Remove(_itemTypeList.Items.FindByValue("Cookie"));                
            }
        }

        /// <summary>
        /// Initialize editor
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="data"></param>
        /// <param name="currentLanguage"></param>
        /// <param name="surveyLanguages"></param>
        /// <param name="isPagePostBack"></param>
        /// <param name="editMode"></param>
        /// <param name="hidePreview"></param>
        public override void Initialize(int templateId, int pagePosition, ItemData data, string currentLanguage, List<string> surveyLanguages, bool isPagePostBack, EditMode editMode, bool hidePreview)
        {
            base.Initialize(templateId, pagePosition, data, currentLanguage, surveyLanguages, isPagePostBack, editMode, hidePreview);

            _activeDisplay.Initialize(data);

            //Set editor properties
            if (TextDecorator != null
                && TextDecorator is LabelledItemTextDecorator
                && data is HiddenItemData)
            {
                _questionTxt.Text = ((LabelledItemTextDecorator)TextDecorator).Text;
                _aliasTxt.Text = data.Alias;

                _variableNameTxt.Text = ((HiddenItemData)data).VariableName;

                if (_itemTypeList.Items.FindByValue(((HiddenItemData)data).VariableSource.ToString()) != null)
                {
                    _itemTypeList.SelectedValue = ((HiddenItemData)data).VariableSource.ToString();
                }
            }
        }

        /// <summary>
        /// Validate the control
        /// </summary>
        /// <returns></returns>
        public override bool Validate()
        {
            bool result = !String.IsNullOrEmpty(_variableNameTxt.Text);
            
            if (result)
            {
                //Only alphanumeric characters checking
                Regex rgx = new Regex(@"^\w+$");
                if (!rgx.IsMatch(_variableNameTxt.Text))
                {
                    _variableNameError.Text = WebTextManager.GetText("/controlText/hiddenItemEditor/invalidNameError");
                    result = false;
                }

                var reservedNames = new HashSet<string> { "iid", "id", "r", "s", "u" };
                if (reservedNames.Contains(_variableNameTxt.Text))
                {
                    _variableNameError.Text = string.Format(WebTextManager.GetText("/controlText/hiddenItemEditor/reservedNameError"), _variableNameTxt.Text);
                    result = false;
                }
            }
            else
            {
                _variableNameError.Text = WebTextManager.GetText("/controlText/hiddenItemEditor/variableNameError");
            }

            _errorPanel.Visible = !result;

            //if everything is OK, return to "Preview"
            if (result)
                _currentTabIndex.Text = "0";
            
            return result;
        }

        /// <summary>
        /// Update data
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            if (TextDecorator != null
                && TextDecorator is LabelledItemTextDecorator
                && ItemData is HiddenItemData)
            {
                ((LabelledItemTextDecorator)TextDecorator).Text = _questionTxt.Text.Trim();
                ItemData.Alias = _aliasTxt.Text.Trim();

                ((HiddenItemData)ItemData).VariableName = _variableNameTxt.Text.Trim();
                ((HiddenItemData)ItemData).VariableSource = (HiddenVariableSource)Enum.Parse(typeof(HiddenVariableSource), _itemTypeList.SelectedValue);
            }
        }
    }
}
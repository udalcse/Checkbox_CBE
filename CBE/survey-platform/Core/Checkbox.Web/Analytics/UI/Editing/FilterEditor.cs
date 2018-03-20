using System;
using System.Linq;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using Checkbox.Forms;
using Checkbox.Forms.Data;
using Checkbox.Security;
using Checkbox.Common;
using Checkbox.Forms.Logic;
using Checkbox.Web.UI.Controls;
using Checkbox.Globalization.Text;
using System.Collections.ObjectModel;
using Checkbox.Forms.Items.Configuration;

namespace Checkbox.Web.Analytics.UI.Editing
{
    /// <summary>
    /// Editor for filters
    /// </summary>
    public class FilterEditor : CompositeControl
    {
        private LocalizedLabelledDropDownList _sourceType;
        private LocalizedLabelledDropDownList _sourceProfileProperties;
        private LocalizedLabelledDropDownList _sourceResponseProperties;
        private LocalizedLabelledDropDownList _sourceItems;
        private LocalizedLabelledEnumDropDownList _operators;
        private LocalizedLabelledDropDownList _values;
        private LocalizedLabelledTextBox _textValue;
        private bool _allowItemChange;

        private readonly ResponseTemplate _responseTemplate;
        private string _languageCode;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="responseTemplate"></param>
        public FilterEditor(ResponseTemplate responseTemplate)
        {
            _responseTemplate = responseTemplate;
            _allowItemChange = true;
            MaxItemTextLength = 128;
        }

        /// <summary>
        /// Get/set max length for text to display for items
        /// </summary>
        public int? MaxItemTextLength { get; set; }

        /// <summary>
        /// Ensure child controls get created on postback
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EnsureChildControls();
        }

        /// <summary>
        /// Get/set whether the source item can be changed
        /// </summary>
        public bool AllowItemChange
        {
            get { return _allowItemChange; }
            set
            {
                EnsureChildControls();
                _allowItemChange = value;
                _sourceItems.Enabled = value;
                _sourceType.Enabled = value;
                _sourceProfileProperties.Enabled = value;
                _sourceResponseProperties.Enabled = value;
            }            
        }


        /// <summary>
        /// Per request cache for items
        /// </summary>
        List<LightweightItemMetaData> _itemList;

        /// <summary>
        /// Populate the list of source filter items
        /// </summary>
        protected virtual void PopulateItemList()
        {
            List<LightweightItemMetaData> itemList = ItemConfigurationManager.ListResponseTemplateItems(
                _responseTemplate.ID.Value,
                int.MaxValue,
                false,
                false,
                true,
                ItemLanguageCode);

            _itemList = itemList;

            foreach (LightweightItemMetaData itemData in itemList)
            {
                //Miss RankOrder item. It isn't useful for branching/conditions.
                if (itemData.ItemType.Equals("RankOrder", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                String text = itemData.GetText(false, ItemLanguageCode);
                if (String.IsNullOrEmpty(text))
                    text = "Item: " + itemData.ItemId;
                _sourceItems.AddItem(new ListItem(Utilities.DecodeAndStripHtml(itemData.PositionText + " " + text, 64), itemData.ItemId.ToString()));

                foreach (int childItemId in itemData.Children)
                {
                    LightweightItemMetaData childItemData = SurveyMetaDataProxy.GetItemData(childItemId, false);

                    if (childItemData != null && childItemData.IsAnswerable)
                    {
                        text = Utilities.DecodeAndStripHtml(childItemData.GetText(false, ItemLanguageCode));
                        if (String.IsNullOrEmpty(text))
                            text = "Item: " + itemData.ItemId;

                        _sourceItems.AddItem(new ListItem(
                                                 "&nbsp;&nbsp;&nbsp;&nbsp;" + Utilities.DecodeAndStripHtml(itemData.PositionText + " " + text, 64),
                                                 string.Format("{0}_{1}", itemData.ItemId, childItemData.ItemId)));

                    }
                }
            }
        }

        /// <summary>
        /// Populate list of source type options
        /// </summary>
        protected virtual void PopulateSourceTypeList()
        {
            _sourceType.AddItem(new ListItem(WebTextManager.GetText("/controlText/filterEditor/question"), "ITEM"));
            _sourceType.AddItem(new ListItem(WebTextManager.GetText("/controlText/filterEditor/userAttribute"), "PROFILE"));
            _sourceType.AddItem(new ListItem(WebTextManager.GetText("/controlText/filterEditor/responseProperty"), "RESPONSE"));
        }

        /// <summary>
        /// Populate the list of response properties
        /// </summary>
        protected virtual void PopulateResponsePropertyList()
        {
            ReadOnlyCollection<string> propertyNames = ResponseProperties.PropertyNames;

            foreach (string propertyName in propertyNames)
            {
                if (!propertyName.Equals("CurrentDateUS", StringComparison.InvariantCultureIgnoreCase)
                    && !propertyName.Equals("CurrentDateROTW", StringComparison.InvariantCultureIgnoreCase)
                    && !propertyName.Equals("CurrentScore", StringComparison.InvariantCultureIgnoreCase)
                    && !propertyName.Equals("TotalPossibleScore", StringComparison.InvariantCultureIgnoreCase))
                {

                    string nameText = WebTextManager.GetText("/responseProperty/" + propertyName + "/text");

                    if (Utilities.IsNullOrEmpty(nameText))
                    {
                        nameText = propertyName;
                    }

                    _sourceResponseProperties.AddItem(new ListItem(nameText, propertyName));
                }
            }
        }

        /// <summary>
        /// Check if it should use datetimepicker
        /// </summary>
        /// <returns></returns>
        public void CheckDateTimePicker()
        {
            if(_sourceType.SelectedValue.Equals("RESPONSE") &&
                (_sourceResponseProperties.SelectedValue.Equals("Ended") ||
                _sourceResponseProperties.SelectedValue.Equals("LastEdit") ||
                _sourceResponseProperties.SelectedValue.Equals("Started")))
            {
                _textValue.Attributes["datetimepicker"] = "true";                
            }
            else
            {
                _textValue.Attributes["datetimepicker"] = "false";
            }
        }

        /// <summary>
        /// Populate the list of profile properties
        /// </summary>
        protected virtual void PopulateProfilePropertyList()
        {
            List<string> propertyList = ProfileManager.ListPropertyNames();

            foreach (string property in propertyList)
            {
                _sourceProfileProperties.AddItem(new ListItem(property, property));
            }
        }

        /// <summary>
        /// Create child controls
        /// </summary>
        protected override void CreateChildControls()
        {
            //Create controls
            _sourceType = new LocalizedLabelledDropDownList
            {
                LabelCssClass = "PrezzaLabel",
                LabelWidth = Unit.Pixel(100),
                ListCssClass = "PrezzaNormal",
                LabelTextID = "/controlText/filterEditor/sourceType",
                ItemSpacing = Unit.Pixel(3)
            };

            _sourceProfileProperties = new LocalizedLabelledDropDownList
            {
                LabelCssClass = "PrezzaLabel",
                LabelWidth = Unit.Pixel(100),
                ListCssClass = "PrezzaNormal",
                LabelTextID = "/controlText/filterEditor/userAttributeName",
                ItemSpacing = Unit.Pixel(3)
            };

            _sourceResponseProperties = new LocalizedLabelledDropDownList
            {
                LabelCssClass = "PrezzaLabel",
                LabelWidth = Unit.Pixel(100),
                ListCssClass = "PrezzaNormal",
                LabelTextID = "/controlText/filterEditor/responsePropertyName",
                ItemSpacing = Unit.Pixel(3)
            };

            _sourceItems = new LocalizedLabelledDropDownList
            {
                LabelCssClass = "PrezzaLabel",
                LabelWidth = Unit.Pixel(100),
                ListCssClass = "PrezzaNormal",
                LabelTextID = "/controlText/filterEditor/sourceItem",
                ItemSpacing = Unit.Pixel(3)
            };

            _operators = new LocalizedLabelledEnumDropDownList
            {
                EnumType = typeof (LogicalOperator),
                LabelCssClass = "PrezzaLabel",
                LabelWidth = Unit.Pixel(100),
                ListCssClass = "PrezzaNormal",
                LabelTextID = "/controlText/filterEditor/operator",
                ItemSpacing = Unit.Pixel(3)
            };

            _values = new LocalizedLabelledDropDownList
            {
                LabelCssClass = "PrezzaLabel",
                LabelWidth = Unit.Pixel(100),
                ListCssClass = "PrezzaNormal",
                LabelTextID = "/controlText/filterEditor/targetValue",
                ItemSpacing = Unit.Pixel(3)
            };

            _textValue = new LocalizedLabelledTextBox
            {
                LabelCssClass = "PrezzaLabel",
                LabelWidth = Unit.Pixel(100),
                TextBoxCssClass = "PrezzaNormal",
                LabelTextID = "/controlText/filterEditor/targetValue",
                ItemSpacing = Unit.Pixel(3)
            };

            //Add options
            PopulateSourceTypeList();
            PopulateItemList();
            PopulateResponsePropertyList();
            PopulateProfilePropertyList();

            //Bind events
            _sourceType.AutoPostBack = true;
            _sourceType.BindSelectedIndexChangedEvent(_sourceType_SelectedIndexChanged);

            _sourceItems.AutoPostBack = true;
            _sourceItems.BindSelectedIndexChangedEvent(_sourceItems_SelectedIndexChanged);

            _operators.AutoPostBack = true;
            _operators.BindSelectedIndexChangedEvent(_operators_SelectedIndexChanged);

            _sourceResponseProperties.AutoPostBack = true;
            _sourceResponseProperties.BindSelectedIndexChangedEvent(_responseProperties_SelectedIndexChanged);

            _sourceProfileProperties.AutoPostBack = true;
            _sourceProfileProperties.BindSelectedIndexChangedEvent(_profileProperties_SelectedIndexChanged);

            //Add controls to control tree
            Controls.Add(_sourceType);
            Controls.Add(_sourceItems);
            Controls.Add(_sourceProfileProperties);
            Controls.Add(_sourceResponseProperties);
            Controls.Add(_operators);
            Controls.Add(_values);
            Controls.Add(_textValue);

            //Update options            
            UpdateItemOptions();

            //Update operators
            UpdateOperators();
        }

        /// <summary>
        /// Get the language code to use for filterable items
        /// </summary>
        private string ItemLanguageCode
        {
            get
            {
                if (_languageCode != null && _languageCode.Trim() != string.Empty)
                {
                    return _languageCode;
                }

                //Try the user language
                foreach (string language in _responseTemplate.LanguageSettings.SupportedLanguages)
                {
                    if (string.Compare(language, WebTextManager.GetUserLanguage(), true) == 0)
                    {
                        _languageCode = language;
                        return _languageCode;
                    }
                }

                //Try the application default language
                foreach (string language in _responseTemplate.LanguageSettings.SupportedLanguages)
                {
                    if (string.Compare(language, TextManager.DefaultLanguage, true) == 0)
                    {
                        _languageCode = language;
                        return _languageCode;
                    }
                }

                //Finally, return the default language
                if (_responseTemplate.LanguageSettings.DefaultLanguage != null && _responseTemplate.LanguageSettings.DefaultLanguage.Trim() != string.Empty)
                {
                    _languageCode = _responseTemplate.LanguageSettings.DefaultLanguage;
                    return _languageCode;
                }
                
                _languageCode = "en-US";
                return _languageCode;
            }
        }

        /// <summary>
        /// Handle changing source type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _sourceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckDateTimePicker();
            UpdateItemOptions();
            _textValue.Text = string.Empty;
        }

        /// <summary>
        /// Handle operator change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _operators_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateItemOptions();
        }

        /// <summary>
        /// Handle operator change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _responseProperties_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckDateTimePicker();
            _textValue.Text = string.Empty;
        }

        /// <summary>
        /// Handle operator change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _profileProperties_SelectedIndexChanged(object sender, EventArgs e)
        {
            _textValue.Text = string.Empty;
        }

        /// <summary>
        /// Handle index change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _sourceItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateItemOptions();
        }

        /// <summary>
        /// Update visibility of item lists
        /// </summary>
        private void UpdateListVisibility()
        {
            _sourceItems.Visible = (_sourceType.SelectedValue == "ITEM");
            _sourceProfileProperties.Visible = (_sourceType.SelectedValue == "PROFILE");
            _sourceResponseProperties.Visible = (_sourceType.SelectedValue == "RESPONSE");

            if (Operator.HasValue)
            {
                if (Operator.Value == LogicalOperator.Answered
                    || Operator.Value == LogicalOperator.NotAnswered
                    || Operator.Value == LogicalOperator.IsNull
                    || Operator.Value == LogicalOperator.IsNotNull)
                {
                    _values.Visible = false;
                    _textValue.Visible = false;
                }
                else
                {
                    _values.Visible = false;
                    _textValue.Visible = true;

                    //Check if value list should be shown instead of text entry
                    if (_sourceItems.Visible)
                    {
                        ItemData sourceItem = GetSelectedItem();

                        if (sourceItem != null && sourceItem is SelectItemData)
                        {
                            if (sourceItem is SliderItemData && (sourceItem as SliderItemData).ValueType == SliderValueType.NumberRange)
                            {
                                _values.Visible = false;
                                _textValue.Visible = true;
                            }
                            else
                            {
                                _values.Visible = true;
                                _textValue.Visible = false;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update option list with selected items
        /// </summary>
        private void UpdateOptionList()
        {
            _values.ClearItems();

            ItemData sourceItem = GetSelectedItem();

            if (sourceItem is SelectItemData)
            {
                foreach (var option in SurveyMetaDataProxy.LoadItemData(sourceItem).Options)
                {
                    _values.AddItem(new ListItem(Utilities.DecodeAndStripHtml(SurveyMetaDataProxy.GetOptionText(sourceItem.ID, option, ItemLanguageCode, false, false)), option.ToString()));
                }
            }
        }

        /// <summary>
        /// Update selected item options
        /// </summary>
        private void UpdateItemOptions()
        {
            //Update visibility returns a boolean indicating if the value selection
            // dropdown needs to be cleared

            //Update control visibility
            UpdateListVisibility();

            if (_values.Visible)
            {
                UpdateOptionList();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            UpdateOperators();
        }

        /// <summary>
        /// Hide or show some operators
        /// </summary>
        public void UpdateOperators()
        {
            _operators.RemoveItem("OperatorNotSpecified");

            if ("ITEM".Equals(SourceType, StringComparison.InvariantCultureIgnoreCase))
            {
                _operators.RemoveItem("IsNull");
                _operators.RemoveItem("IsNotNull");

                _operators.AddItem(new ListItem(WebTextManager.GetText("/enum/logicalOperator/answered"), LogicalOperator.Answered.ToString()));
                _operators.AddItem(new ListItem(WebTextManager.GetText("/enum/logicalOperator/notAnswered"), LogicalOperator.NotAnswered.ToString()));

                string itemType = null;
                string[] operatorsToExclude = new string[] { 
                    LogicalOperator.Contains.ToString(), LogicalOperator.DoesNotContain.ToString(),
                    LogicalOperator.Answered.ToString(), LogicalOperator.NotAnswered.ToString()
                };

                if (_sourceItems != null && _sourceItems.ItemCount > 0 && _itemList != null && SourceItemID != null)
                {
                    itemType = (from i in _itemList where i.ItemId == SourceItemID.Value select i.ItemType).FirstOrDefault();
                }

                if ("Slider".Equals(itemType, StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (var o in operatorsToExclude)
                    {
                        _operators.RemoveItem(o);
                    }
                }
                else
                {
                    foreach (var o in operatorsToExclude)
                    {
                        _operators.AddItem(new ListItem(WebTextManager.GetText("/enum/logicalOperator/" + o), o));
                    }
                }
            }
            else
            {
                _operators.RemoveItem("Answered");
                _operators.RemoveItem("NotAnswered");

                _operators.AddItem(new ListItem(WebTextManager.GetText("/enum/logicalOperator/contains"), LogicalOperator.Contains.ToString()));
                _operators.AddItem(new ListItem(WebTextManager.GetText("/enum/logicalOperator/doesNotContain"), LogicalOperator.DoesNotContain.ToString()));
                _operators.AddItem(new ListItem(WebTextManager.GetText("/enum/logicalOperator/isNull"), LogicalOperator.IsNull.ToString()));
                _operators.AddItem(new ListItem(WebTextManager.GetText("/enum/logicalOperator/isNotNull"), LogicalOperator.IsNotNull.ToString()));
            }

        }

        /// <summary>
        /// Get the selected item from the list
        /// </summary>
        /// <returns></returns>
        private ItemData GetSelectedItem()
        {
            if (!SourceItemID.HasValue)
            {
                return null;
            }

            return ItemConfigurationManager.GetConfigurationData(SourceItemID.Value);
        }

        /// <summary>
        /// Get/set the source type
        /// </summary>
        public string SourceType
        {
            get
            {
                EnsureChildControls();
                return _sourceType.SelectedValue;
            }

            set
            {
                if (value != null)
                {
                    EnsureChildControls();
                    _sourceType.SelectedValue = value.ToUpper();
                    UpdateItemOptions();
                }
            }
        }

        /// <summary>
        /// Get/set the source profile property
        /// </summary>
        public string SourceProfileProperty
        {
            get
            {
                EnsureChildControls();
                return _sourceProfileProperties.SelectedValue;
            }

            set
            {
                EnsureChildControls();
                _sourceProfileProperties.SelectedValue = value;
            }
        }

        /// <summary>
        /// Get/set the source response property
        /// </summary>
        public string SourceResponseProperty
        {
            get
            {
                EnsureChildControls();
                return _sourceResponseProperties.SelectedValue;
            }

            set
            {
                EnsureChildControls();
                _sourceResponseProperties.SelectedValue = value;
            }
        }

        /// <summary>
        /// Get/set the source item
        /// </summary>
        public int? SourceItemID
        {
            get
            {
                EnsureChildControls();
                return Utilities.AsInt(_sourceItems.SelectedValue);
            }

            set
            {
                EnsureChildControls();
                _sourceItems.SelectedValue = value.ToString();
                UpdateItemOptions();
                UpdateOperators();
            }
        }

        /// <summary>
        /// Get/set the logical operator for the item
        /// </summary>
        public LogicalOperator? Operator
        {
            get
            {
                EnsureChildControls();

                if (_operators != null && _operators.SelectedValue.Trim() != string.Empty)
                {
                    return (LogicalOperator)Enum.Parse(typeof(LogicalOperator), _operators.SelectedValue);
                }
                
                return null;
            }

            set
            {
                EnsureChildControls();
                _operators.SelectedValue = value.ToString();
                UpdateItemOptions();
            }
        }


        /// <summary>
        /// Return a boolean indicating if the source item list is empty
        /// </summary>
        public bool SourceItemListEmpty
        {
            get 
            {
                EnsureChildControls();

                return (_sourceItems.ItemCount == 0);
            }
        }
        
        /// <summary>
        /// Get/set the selected value
        /// </summary>
        public string Value
        {
            get
            {
                EnsureChildControls();

                if (_textValue.Visible)
                {
                    return _textValue.Text;
                }
                
                if (_values.Visible)
                {
                    return _values.SelectedValue;
                }
                return null;
            }

            set
            {
                EnsureChildControls();

                if (_textValue.Visible)
                {
                    _textValue.Text = value;
                }
                else
                {
                    _values.SelectedValue = value;
                }
            }
        }
    }
}

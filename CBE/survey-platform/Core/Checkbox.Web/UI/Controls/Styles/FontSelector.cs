using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Checkbox.Common;
using Checkbox.Styles;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Web.UI.Controls.Validation;

namespace Checkbox.Web.UI.Controls.Styles
{
    /// <summary>
    /// Font selector
    /// </summary>
    public class FontSelector : LocalizedLabelledControl, IDataEditorControl<StyleTemplate>
    {
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler BuildStyleClick;

        private XmlBoundDropDownList _familyList;
        private XmlBoundDropDownList _styleList;
        private XmlBoundDropDownList _fontSizeList;
        private TextBox _colorInput;
        private ColorPicker _colorPicker;
        private CalloutColorValidator _colorValidator;

        private LinkButton _styleBuilderLink;

        /// <summary>
        /// Get the font-family list
        /// </summary>
        protected XmlBoundDropDownList FamilyList
        {
            get
            {
                if (_familyList == null)
                {
                    _familyList = new XmlBoundDropDownList 
                    {
                        ID = "FontFamilyList", 
                        DataFile = "~/Resources/CodeDependentResources.xml", 
                        XPath = "/CodeDependentResources/StyleFonts/FontName", 
                        DataValueField = "Value", 
                        DataTextField = "Text"
                    };
                }
                return _familyList;
            }
        }

        /// <summary>
        /// Get the font style list
        /// </summary>
        protected XmlBoundDropDownList StyleList
        {
            get
            {
                if (_styleList == null)
                {
                    _styleList = new XmlBoundDropDownList 
                    {
                        ID = "FontStyleList", 
                        DataFile = "~/Resources/CodeDependentResources.xml", 
                        XPath = "/CodeDependentResources/FontStyles/FontStyle", 
                        DataValueField = "Value", 
                        DataTextField = "Text"
                    };
                }
                return _styleList;
            }
        }

        /// <summary>
        /// Get the font size list
        /// </summary>
        protected XmlBoundDropDownList FontSizeList
        {
            get
            {
                if (_fontSizeList == null)
                {
                    _fontSizeList = new XmlBoundDropDownList 
                    {
                        ID = "FontSizeList", 
                        DataFile = "~/Resources/CodeDependentResources.xml", 
                        XPath = "/CodeDependentResources/FontSizes/FontSize", 
                        DataValueField = "Value", 
                        DataTextField = "Text"
                    };
                }
                return _fontSizeList;
            }
        }

        /// <summary>
        /// Get the color input
        /// </summary>
        protected TextBox ColorInput
        {
            get
            {
                if (_colorInput == null)
                {
                    _colorInput = new TextBox {ID = "FontColorInput", Width = Unit.Pixel(60)};
                }

                return _colorInput;
            }
        }

        /// <summary>
        /// Get the color picker
        /// </summary>
        protected ColorPicker ColorPicker
        {
            get
            {
                if (_colorPicker == null)
                {
                    _colorPicker = new ColorPicker {ColorPickerField = ColorInput, ID = "FontColorPicker"};
                }

                return _colorPicker;
            }
        }

        /// <summary>
        /// Get/set enable style builder flag
        /// </summary>
        public bool EnableStyleBuilder { get; set; }

        /// <summary>
        /// Get/set skin id for drop down lists
        /// </summary>
        public string DropDownListSkinID
        {
            get { return FamilyList.SkinID; }
            set
            {
                FamilyList.SkinID = value;
                StyleList.SkinID = value;
                FontSizeList.SkinID = value;
            }
        }

        /// <summary>
        /// Get/set skin id for text input
        /// </summary>
        public string TextBoxSkinID
        {
            get { return ColorInput.SkinID; }
            set { ColorInput.SkinID = value; }
        }

        /// <summary>
        /// Get/set font family
        /// </summary>
        public string Family
        {
            get { return FamilyList.SelectedValue; }
            set { FamilyList.SelectedValue = value; }
        }

        /// <summary>
        /// Get/set font style
        /// </summary>
        public string FontStyle
        {
            get { return StyleList.SelectedValue; }
            set { StyleList.SelectedValue = value; }
        }

        /// <summary>
        /// Get/set font size
        /// </summary>
        public string Size
        {
            get { return FontSizeList.SelectedValue; }
            set { FontSizeList.SelectedValue = value; }
        }

        /// <summary>
        /// Get/set font color
        /// </summary>
        public string Color
        {
            get { return ColorInput.Text; }
            set { ColorInput.Text = value; }
        }

        /// <summary>
        /// Get/set css element name
        /// </summary>
        public string ElementName { get; set; }

        /// <summary>
        /// Get/set an additional element name to update.  Necessary for
        /// updating matrix fonts.
        /// </summary>
        public string AltElementName { get; set; }

        /// <summary>
        /// Get the input control
        /// </summary>
        /// <returns></returns>
        protected override Control GetInputControl()
        {
            Panel p = new Panel();

            _colorValidator = new CalloutColorValidator 
            {
                ID = "ColorValidator", 
                ControlToValidate = _colorInput.ID, 
                TextID = "/pageText/editStyle.aspx/invalidColor", 
                SubTextID = "/pageText/editStyle.aspx/invalidColorText"
            };

            p.Controls.Add(FamilyList);
            p.Controls.Add(StyleList);
            p.Controls.Add(FontSizeList);
            p.Controls.Add(ColorInput);
            p.Controls.Add(ColorPicker);
            p.Controls.Add(_colorValidator);

            return p;
        }

        /// <summary>
        /// Get the label control
        /// </summary>
        public override Control LabelControl
        {
            get
            {
                if (EnableStyleBuilder)
                {
                    if (_styleBuilderLink == null)
                    {
                        _styleBuilderLink = new LinkButton 
                        {
                            ToolTip = WebTextManager.GetText("/controlText/fontSelector/buildStyle"), 
                            SkinID = "FontSelector"
                        };
                    }

                    return _styleBuilderLink;
                }
                return base.LabelControl;
            }
            set
            {
                base.LabelControl = value;
            }
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            EnsureChildControls();

            if (EnableStyleBuilder && LabelControl != null && LabelControl is LinkButton)
            {
                ((LinkButton)LabelControl).Text = GetLabelText();
                ((LinkButton)LabelControl).Click += _styleBuilderLink_Click;
            }

            base.OnInit(e);
        }

        /// <summary>
        /// Handle link click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _styleBuilderLink_Click(object sender, EventArgs e)
        {
            if (BuildStyleClick != null)
            {
                BuildStyleClick(this, e);
            }
        }

        #region IDataEditorControl<StyleTemplate> Members

        /// <summary>
        /// Bind control to style template
        /// </summary>
        /// <param name="datasource"></param>
        public void BindToDataSource(StyleTemplate datasource)
        {
            //Call databind to bind lists
            DataBind();

            //Add "Empty" and "Custom" Entries
            _familyList.Items.Add(new ListItem(WebTextManager.GetText("/controlText/fontSelector/none"), "None"));
            _familyList.Items.Add(new ListItem(WebTextManager.GetText("/controlText/fontSelector/custom"), "Custom"));

            _fontSizeList.Items.Add(new ListItem(WebTextManager.GetText("/controlText/fontSelector/none"), "None"));
            _fontSizeList.Items.Add(new ListItem(WebTextManager.GetText("/controlText/fontSelector/custom"), "Custom"));

            //Bind values to style
            string fontWeight = datasource.GetElementProperty(ElementName, "font-weight");
            string textDecoration = datasource.GetElementProperty(ElementName, "text-decoration");
            string fontStyle = datasource.GetElementProperty(ElementName, "font-style");
            string fontSize = datasource.GetElementProperty(ElementName, "font-size");
            string fontFamily = datasource.GetElementProperty(ElementName, "font-family");

            //Handle potential custom values for font size/family
            if (Utilities.IsNullOrEmpty(fontSize))
            {
                _fontSizeList.SelectedValue = "None";
            }
            else if (_fontSizeList.Items.FindByValue(fontSize) == null)
            {
                _fontSizeList.SelectedValue = "Custom";
            }
            else
            {
                _fontSizeList.SelectedValue = fontSize;
            }

            if (Utilities.IsNullOrEmpty(fontFamily))
            {
                _familyList.SelectedValue = "None";
            }
            else if (_familyList.Items.FindByValue(fontFamily) == null)
            {
                _familyList.SelectedValue = "Custom";
            }
            else
            {
                _familyList.SelectedValue = fontFamily;
            }

            _styleList.SelectedValue = fontWeight + "_" + textDecoration + "_" + fontStyle;
            _colorInput.Text = datasource.GetElementProperty(ElementName, "color");
            _colorPicker.Text = _colorInput.Text;
        }

        /// <summary>
        /// Update the data source
        /// </summary>
        public void UpdateDataSource(StyleTemplate dataSource)
        {
            //Handle None and Custom cases for templates
            if (_familyList.SelectedValue == "None")
            {
                SetElementPropertyValue(dataSource, "font-family", string.Empty);
            }
            else if (_familyList.SelectedValue != "Custom")
            {
                SetElementPropertyValue(dataSource, "font-family", _familyList.SelectedValue);
            }

            if (_fontSizeList.SelectedValue == "None")
            {
                SetElementPropertyValue(dataSource, "font-size", string.Empty);
            }
            else if (_fontSizeList.SelectedValue != "Custom")
            {
                SetElementPropertyValue(dataSource, "font-size", _fontSizeList.SelectedValue);
            }

            SetElementPropertyValue(dataSource, "color", _colorInput.Text);

            string[] styles = _styleList.SelectedValue.Split(new[] { "_" }, StringSplitOptions.None);

            string fontWeight = string.Empty;
            string fontStyle = string.Empty;
            string textDecoration = string.Empty;

            if (styles.Length == 3)
            {
                fontWeight = styles[0];
                textDecoration = styles[1];
                fontStyle = styles[2];
            }

            SetElementPropertyValue(dataSource, "font-weight", fontWeight);
            SetElementPropertyValue(dataSource, "text-decoration", textDecoration);
            SetElementPropertyValue(dataSource, "font-style", fontStyle);
        }

        /// <summary>
        /// Set a style element property value
        /// </summary>
        /// <param name="styleTemplate"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        private void SetElementPropertyValue(StyleTemplate styleTemplate, string propertyName, string value)
        {
            string[] altElementNames = new string[] { };

            if (Utilities.IsNotNullOrEmpty(AltElementName))
            {
                altElementNames = AltElementName.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            }

            if (Utilities.IsNullOrEmpty(value))
            {
                styleTemplate.RemoveElementStyleProperty(ElementName, propertyName);

                foreach(string altElementName in altElementNames)
                {
                    styleTemplate.RemoveElementStyleProperty(altElementName, propertyName);
                }
            }
            else
            {
                styleTemplate.SetElementStyleProperty(ElementName, propertyName, value);

                foreach (string altElementName in altElementNames)
                {
                    styleTemplate.SetElementStyleProperty(altElementName, propertyName, value);
                }
            }
        }

        /// <summary>
        /// Validate control inputs
        /// </summary>
        /// <returns></returns>
        public bool ValidateInputs()
        {
            _colorValidator.Validate();

            return _colorValidator.IsValid;
        }

        #endregion

        /// <summary>
        /// Override enabled to show/hide color picker
        /// </summary>
        public override bool Enabled
        {
            get { return base.Enabled; }
            set
            {
                ColorPicker.Visible = value;
                base.Enabled = false;
            }
        }
    }
}

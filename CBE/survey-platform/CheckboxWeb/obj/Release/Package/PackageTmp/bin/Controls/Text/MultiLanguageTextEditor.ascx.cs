using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;

namespace CheckboxWeb.Controls.Text
{
    /// <summary>
    /// Container class for text item
    /// </summary>
    public class TextItem
    {
        /// <summary>
        /// <summary>
        /// Width of input, in pixels, for single line text input.
        /// </summary>
        [PersistenceMode(PersistenceMode.Attribute)]
        public int? InputWidth { get; set; }

        /// <summary>
        /// Get/set text id for prompt
        /// </summary>
        [PersistenceMode(PersistenceMode.Attribute)]
        public string LabelTextId { get; set; }

        /// <summary>
        /// ID of text to edit
        /// </summary>
        [PersistenceMode(PersistenceMode.Attribute)]
        public string TextId { get; set; }

        /// <summary>
        /// Text value to use in place of loading directly from text manager.  If value is 
        /// NULL, value will be pulled from text manager.  For all other values, including 
        /// string.empty, editor will use this value instead.
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public string TextValue { get; set; }
    }

    /// <summary>
    /// Simple control to encapsulate some basic multilanguage text editing capabilities
    /// </summary>
    [ParseChildren(true, "TextItems")]
    public partial class MultiLanguageTextEditor : Checkbox.Web.Common.UserControlBase
    {
        private Dictionary<string, TextBox> _inputTextBoxes;

        /// <summary>
        /// Get input text boxes
        /// </summary>
        private Dictionary<string, TextBox> TextBoxes
        {
            get
            {
                if (_inputTextBoxes == null)
                {
                    _inputTextBoxes = new Dictionary<string, TextBox>();
                }

                return _inputTextBoxes;
            }
        }

        /// <summary>
        /// Get/set language code for current view
        /// </summary>
        [PersistenceMode(PersistenceMode.Attribute)]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Get/set alternate languages for editor
        /// </summary>
        public List<string> AlternateLanguages { get; set; }

        /// <summary>
        /// Text items to edit in the control.
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<TextItem> TextItems { get; set; }

        /// <summary>
        /// Get/set # of 960 grid columns for label to occupy.
        /// </summary>
        [PersistenceMode(PersistenceMode.Attribute)]
        public int? LabelGridColumns { get; set; }

        /// <summary>
        /// Css class for label.  If used, 960 grid will not be  used
        /// </summary>
        [PersistenceMode(PersistenceMode.Attribute)]
        public string LabelContainerCssClass { get; set; }

        /// <summary>
        /// Css class for label.  If used, 960 grid will not be  used
        /// </summary>
        [PersistenceMode(PersistenceMode.Attribute)]
        public string InputContainerCssClass { get; set; }

        /// <summary>
        /// Initialize text items collection
        /// </summary>
        public MultiLanguageTextEditor()
        {
            TextItems = new List<TextItem>();
        }

        /// <summary>
        /// Override init to get text boxes for use later
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _editorRepeater.ItemDataBound += _editorRepeater_ItemDataBound;
        }

        /// <summary>
        /// Bind repeater to list
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            TextBoxes.Clear();

            _editorRepeater.DataSource = TextItems;
            _editorRepeater.DataBind();
        }

        /// <summary>
        /// Handle item databound to store reference to text box for easier lookup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _editorRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item != null)
            {
                TextBox textBox = e.Item.FindControl("_inputTxt") as TextBox;

                if (textBox != null
                    && Utilities.IsNotNullOrEmpty(textBox.Attributes["TextId"]))
                {
                    TextBoxes[textBox.Attributes["TextId"]] = textBox;
                }
            }
        }

        /// <summary>
        /// Get text values in a dictinary keyed by text id
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetTextValues()
        {
            Dictionary<string, string> outValues = new Dictionary<string, string>();

            foreach (string textId in TextBoxes.Keys)
            {
                outValues[textId] = TextBoxes[textId].Text;
            }

            return outValues;
        }
    }
}
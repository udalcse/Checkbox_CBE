using System.Web.UI;
using System.Web.UI.WebControls;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Text box control with localizable label.
    /// </summary>
    public class LocalizedLabelledTextBox : LocalizedLabelledControl, ITextControl
    {
        private TextBox _textBox;


        /// <summary>
        /// Get/set the input control
        /// </summary>
        public override Control InputControl
        {
            get { return TextBox; }
            set { }          
        }

        /// <summary>
        /// Get the text box
        /// </summary>
        public TextBox TextBox
        {
            get
            {
                if (_textBox == null)
                {
                    _textBox = new TextBox();
                }

                return _textBox;
            }
        }

        /// <summary>
        /// Get/set text mode
        /// </summary>
        public TextBoxMode TextMode
        {
            get { return TextBox.TextMode; }
            set { TextBox.TextMode = value; }
        }

        /// <summary>
        /// Get/set text max length
        /// </summary>
        public int MaxLength
        {
            get { return TextBox.MaxLength; }
            set { TextBox.MaxLength = value; }
        }

        /// <summary>
        /// Get/set text
        /// </summary>
        public string Text
        {
            get { return TextBox.Text; }
            set { TextBox.Text = value; }
        }

        /// <summary>
        /// Get/set rows
        /// </summary>
        public int Rows
        {
            get { return TextBox.Rows; }
            set { TextBox.Rows = value; }
        }

        /// <summary>
        /// Get/set columns
        /// </summary>
        public int Columns
        {
            get { return TextBox.Columns; }
            set { TextBox.Columns = value; }
        }

        /// <summary>
        /// Get/set text box width
        /// </summary>
        public Unit TextBoxWidth
        {
            get { return TextBox.Width; }
            set { TextBox.Width = value; }
        }

        /// <summary>
        /// Get/set autocomplete type
        /// </summary>
        public AutoCompleteType AutoCompleteType
        {
            get { return TextBox.AutoCompleteType; }
            set { TextBox.AutoCompleteType = value; }
        }

        /// <summary>
        /// Get/set text box id
        /// </summary>
        public string TextBoxID
        {
            get { return TextBox.ID; }
            set { TextBox.ID = value; }
        }

        /// <summary>
        /// Get/set text box css class
        /// </summary>
        public string TextBoxCssClass
        {
            get { return TextBox.CssClass; }
            set { TextBox.CssClass = value; }
        }

        /// <summary>
        /// Get/set text box skin id
        /// </summary>
        public string TextBoxSkinID
        {
            get { return TextBox.SkinID; }
            set { TextBox.SkinID = value; }
        }
    }
}

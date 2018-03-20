using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Control for rendering error messages
    /// </summary>
    public class ErrorMessage : Common.WebControlBase
    {
        private Exception _exception;

        private Label _messageLbl;
        private Label _subMessageLbl;

        /// <summary>
        /// Constructor
        /// </summary>
        public ErrorMessage()
        {
            Visible = false;
        }

        /// <summary>
        /// Get/set the message text
        /// </summary>
        public string Text
        {
            get 
            {
                EnsureChildControls();
                return _messageLbl.Text; 
            }
            set 
            {
                EnsureChildControls();
                _messageLbl.Text = value; 
            }
        }

        /// <summary>
        /// Get/set exception value
        /// </summary>
        public Exception Exception
        {
            get 
            { 
                return _exception; 
            }
            set 
            {
                EnsureChildControls();

                _exception = value;

                _subMessageLbl.Text = _exception != null ? _exception.Message : string.Empty;
            }
        }

        /// <summary>
        /// Get/set the error message
        /// </summary>
        public string Message
        {
            get
            {
                EnsureChildControls();
                return _messageLbl.Text;
            }

            set
            {
                EnsureChildControls();
                _messageLbl.Text = value;
            }
        }

        /// <summary>
        /// Get/set the sub-message
        /// </summary>
        public string SubMessage
        {
            get
            {
                EnsureChildControls();
                return _subMessageLbl.Text;
            }

            set
            {
                EnsureChildControls();
                _subMessageLbl.Text = value;
            }
        }

        /// <summary>
        /// Create child render controls
        /// </summary>
        protected override void CreateChildControls()
        {
            Panel container = new Panel {CssClass = "ErrorMessage"};

            Image img = new Image {
                ImageUrl = ("~/App_Themes/CheckboxTheme/Images/warning.gif"), 
                ToolTip = WebTextManager.GetText("/controlText/errorMessage/error")};

            _messageLbl = new Label();
            _messageLbl.Style.Add("color", "red");
            _messageLbl.Style.Add("font", "12px Verdana bold");

            _subMessageLbl = new Label();
            _subMessageLbl.Style.Add("color", "black");
            _subMessageLbl.Style.Add("font", "10px Verdana");


            container.Controls.Add(new LiteralControl("<table cellspacing=\"0\" cellpadding=\"3\"><tr valid=\"middle\">"));
            container.Controls.Add(new LiteralControl("<td>"));
            container.Controls.Add(img);
            container.Controls.Add(new LiteralControl("</td><td>"));
            container.Controls.Add(_messageLbl);
            container.Controls.Add(new LiteralControl("<br />"));
            container.Controls.Add(_subMessageLbl);
            container.Controls.Add(new LiteralControl("</td></tr></table>"));

            Controls.Add(container);
        }
    }
}

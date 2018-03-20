using System.Web.UI;
using System.Web.UI.WebControls;

namespace Checkbox.Web.UI.Controls.Validation
{
    /// <summary>
    /// Base validator class that displays a callout when an error occurs
    /// </summary>
    public abstract class WarningCalloutValidator : BaseValidator
    {
        private WarningCallout _callout;

        /// <summary>
        /// Ensure child controls on init
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);

            EnsureChildControls();

            Style[HtmlTextWriterStyle.Display] = "inline";
            Display = ValidatorDisplay.Dynamic;

            if (!Width.IsEmpty)
            {
                _callout.Style[HtmlTextWriterStyle.Width] = Width.ToString();
            }

            if (!Height.IsEmpty)
            {
                _callout.Style[HtmlTextWriterStyle.Height] = Height.ToString();
            }
        }

        /// <summary>
        /// Override the tag key so this control renders as a div, not a span
        /// </summary>
        protected override HtmlTextWriterTag TagKey
        {
            get { return HtmlTextWriterTag.Div; }
        }

        /// <summary>
        /// Create the child controls
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            _callout = new WarningCallout();

            if (!Width.IsEmpty)
            {
                _callout.Style[HtmlTextWriterStyle.Width] = Width.ToString();
            }

            if (!Height.IsEmpty)
            {
                _callout.Style[HtmlTextWriterStyle.Height] = Height.ToString();
            }

            _callout.Style[HtmlTextWriterStyle.Display] = "none";

            Controls.Add(_callout);
        }

        /// <summary>
        /// Get/set the child controls
        /// </summary>
        public string TextID
        {
            get
            {
                EnsureChildControls();
                return _callout.TextID;
            }
            set
            {
                EnsureChildControls();
                _callout.TextID = value;
            }
        }

        /// <summary>
        /// Get/set the text id of the sub text
        /// </summary>
        public string SubTextID
        {
            get
            {
                EnsureChildControls();
                return _callout.SubTextID;
            }
            set
            {
                EnsureChildControls();
                _callout.SubTextID = value;
            }
        }

        /// <summary>
        /// Get/set callout text
        /// </summary>
        public new string Text
        {
            get
            {
                EnsureChildControls();
                return _callout.Text;
            }

            set
            {
                EnsureChildControls();
                _callout.Text = value;
            }
        }

        /// <summary>
        /// Get/set callout sub text
        /// </summary>
        public string SubText
        {
            get
            {
                EnsureChildControls();
                return _callout.SubText;
            }

            set
            {
                EnsureChildControls();
                _callout.SubText = value;
            }
        }
        
        /// <summary>
        /// Evaluate
        /// </summary>
        /// <returns></returns>
        protected override bool EvaluateIsValid()
        {
            bool validated = ValidateInput();

            _callout.Style[HtmlTextWriterStyle.Display] = validated ? "none" : "block";
            
            return validated;
        }

        /// <summary>
        /// Validate the input
        /// </summary>
        /// <returns></returns>
        protected abstract bool ValidateInput();
    }
}

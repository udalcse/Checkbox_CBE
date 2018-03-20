using System;
using System.Drawing;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Warning callout that extends ML callout to specify some defaults
    /// </summary>
    public class WarningCallout : MultiLanguageCallout
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public WarningCallout() : base()
        {
            BackColor = Color.LemonChiffon;
            BorderColor = Color.Black;
        }

        /// <summary>
        /// Name of the image
        /// </summary>
        public override string ImageName
        {
            get { return "warning.gif"; }
            set { }
        }

        /// <summary>
        /// Callout text css class
        /// </summary>
        public override string TextCssClass
        {
            get { return "ErrorMessage"; }
            set { }
        }

        /// <summary>
        /// Callout subtext css class
        /// </summary>
        public override string SubTextCssClass
        {
            get { return "PrezzaNormal"; }
            set { }
        }
    }
}

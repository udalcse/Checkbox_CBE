using System;
using System.Web.UI.WebControls;


namespace Checkbox.Web.UI.Controls
{

	/// <summary>
	/// Progress bar class which will be used.
	/// </summary>
	public class ProgressBar : Panel
	{
        private Panel _innerPanel;

        /// <summary>
        /// Get the inner panel
        /// </summary>
        private Panel InnerPanel
        {
            get { return _innerPanel ?? (_innerPanel = new Panel {CssClass = "ProgressBarInner"}); }
        }

		/// <summary>
		/// Get/set the percent width of the progress bar
		/// </summary>
        public double Progress
        {
            get { return InnerPanel.Width.Value; }
            set
            {
                InnerPanel.Width = Unit.Percentage(Math.Max(0, value));
            }
        }

        /// <summary>
        /// Create child controls
        /// </summary>
        protected override void CreateChildControls()
        {
            CssClass = "ProgressBar";
            Controls.Add(InnerPanel);
        }
	}
}

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Web.Page;

namespace CheckboxWeb
{
    public partial class DetailList : BaseMasterPage
    {
        public Unit? LeftPanelWidth { get; set; }

        /// <summary>
        /// Override panel width
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (LeftPanelWidth.HasValue)
            {
                _leftPanel.Width = LeftPanelWidth.Value;
            }
        }

        /// <summary>
        /// Set page title
        /// </summary>
        /// <param name="title"></param>
        public override void SetTitle(string title)
        {
            var titleControl = new LiteralControl();
            titleControl.Text += "<h3 class=\"pageTitle\">";
            titleControl.Text += title;
            titleControl.Text += "</h3>";

            _titlePlace.Controls.Clear();
            _titlePlace.Controls.Add(titleControl);
        }

        /// <summary>
        /// Get client id of right panel
        /// </summary>
        public string RightPaneClientId { get { return _rightPanel.ClientID; } }

        /// <summary>
        /// Hide search in master page
        /// </summary>
        public void HideSearch()
        {
            Master.HideSearch();
        }

        public void ShowBackButton(string navigateUrl, bool isVisible)
        {
            //_backButton.HRef = navigateUrl;
            //_backButton.Visible = isVisible;
        }
    }
}
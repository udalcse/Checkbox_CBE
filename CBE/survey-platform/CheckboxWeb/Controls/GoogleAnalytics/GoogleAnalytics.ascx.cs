using Checkbox.Web.Common;

namespace CheckboxWeb.Controls.GoogleAnalytics
{
    public partial class GoogleAnalytics : UserControlBase
    {
        protected string GoogleAnalyticsTrackID { set; get; }

        public void Initialize(string googleAnalyticsTrackID)
        {
            GoogleAnalyticsTrackID = googleAnalyticsTrackID;
        }

    }
}
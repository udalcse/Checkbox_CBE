using System;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Settings
{
    public partial class OptOutPreferences : SettingsPage
    {
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            Master.OkClick += Master_OkClick;

            _greetingsTextbox.Text = WebTextManager.GetText("/pageText/optOut.aspx/optOutText");
            _blockSurveyTextbox.Text = WebTextManager.GetText("/pageText/optOut.aspx/dontWantThisSurvey");
            _blockSenderTextbox.Text = WebTextManager.GetText("/pageText/optOut.aspx/dontWantThisSender");
            _spamTextbox.Text = WebTextManager.GetText("/pageText/optOut.aspx/thisIsSpam");
            _thankYouTextbox.Text = WebTextManager.GetText("/pageText/optOut.aspx/thanks");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        void Master_OkClick(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(_greetingsTextbox.Text))
            {
                TextManager.SetText("/pageText/optOut.aspx/optOutText", _greetingsTextbox.Text);
            }
            if (!String.IsNullOrEmpty(_blockSurveyTextbox.Text))
            {
                TextManager.SetText("/pageText/optOut.aspx/dontWantThisSurvey", _blockSurveyTextbox.Text);
            }
            if (!String.IsNullOrEmpty(_blockSenderTextbox.Text))
            {
                TextManager.SetText("/pageText/optOut.aspx/dontWantThisSender", _blockSenderTextbox.Text);
            }
            if (!String.IsNullOrEmpty(_spamTextbox.Text))
            {
                TextManager.SetText("/pageText/optOut.aspx/thisIsSpam", _spamTextbox.Text);
            }
            if (!String.IsNullOrEmpty(_thankYouTextbox.Text))
            {
                TextManager.SetText("/pageText/optOut.aspx/thanks", _thankYouTextbox.Text);
            }
        }
    }
}
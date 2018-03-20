using System;
using Checkbox.Forms;
using Checkbox.Invitations;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Styles;
using System.Web.UI;

namespace CheckboxWeb
{
    /// <summary>
    /// 
    /// </summary>
    public partial class OptOut : Checkbox.Web.Page.BasePage
    {
        private ResponseTemplate _responseTemplate;
        /// <summary>
        /// Survey
        /// </summary>
        protected ResponseTemplate Survey
        {
            get
            {
                if (_responseTemplate == null)
                {
                    string s = Request.Params["s"];
                    Guid surveyGuid;
                    if (!Guid.TryParse(s, out surveyGuid))
                        throw new Exception("\"s\" parameter from query string cannot be parsed.");

                    _responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyGuid);
                }
                return _responseTemplate;
            }
        }

        /// <summary>
        /// Title or name if title is empty
        /// </summary>
        public string SurveyTitle
        {
            get
            {
                string title = WebTextManager.GetText(Survey.TitleTextID);
                if (string.IsNullOrEmpty(title))
                    return Survey.Name;
                return title;
            }            
        }

        protected Recipient _recipient;
        /// <summary>
        /// Recipient data
        /// </summary>
        protected Recipient Recipient
        {            
            get                
            {
                if (_recipient == null)
                {
                    string i = Request.Params["i"];
                    Guid invitee;
                    if (!Guid.TryParse(i, out invitee))
                        throw new Exception("\"i\" parameter from query string cannot be parsed.");

                    _recipient = InvitationManager.GetRecipientByGuid(invitee);
                }
                return _recipient;
            }
        }


        /// <summary>
        /// Builds survey URL
        /// </summary>
        private string SurveyUrl
        {
            get
            {
                return string.Format(ApplicationManager.ApplicationRoot + "/Survey.aspx?s={0}&i={1}", Request["s"], Request["i"]);
            }
        }

        protected bool IsBrowserMobile
        {
            get { return WebUtilities.IsBrowserMobile(Request); }
        }

        /// <summary>
        /// Apply the survey's style template.
        /// </summary>
        /// <param name="styleTemplateId"></param>
        private void ApplyStyleTemplate()
        {
            if (!Survey.StyleSettings.StyleTemplateId.HasValue)
                return;

            StyleTemplate st = StyleTemplateManager.GetStyleTemplate(Survey.StyleSettings.StyleTemplateId.Value);

            if (st == null)
                return;

            _surveyStylePlace.Controls.Clear();
            _surveyStylePlace.Controls.Add(new LiteralControl("<style type=\"text/css\">" + st.GetCss() + "</style>"));
        }

        private void ApplyScriptsInclude()
        {
            if (IsBrowserMobile)
            {
                _scriptsPlace.Controls.Add(new LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" media=\"all\" href=\"CheckboxHandheld.css\" />"));
                _scriptsPlace.Controls.Add(new LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" media=\"all\" href=\"HandheldSurveyStyles.css\" />"));
                _scriptsPlace.Controls.Add(new LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" media=\"all\" href=\"Resources/mobile_themes/default/jquery.mobile-latest.css\" />"));

                _scriptsPlace.Controls.Add(new LiteralControl("<script type=\"text/javascript\" src=\"Resources/jquery.mobile-latest.min.js\"></script>"));
            }
            else
            {
                _scriptsPlace.Controls.Add(new LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" media=\"all\" href=\"CheckboxScreen.css\" />"));                
            }
        }

        /// <summary>
        /// Onload Handler
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ApplyStyleTemplate();
            ApplyScriptsInclude();

            _takeSurvey.NavigateUrl = SurveyUrl;
            _takeSurvey.Attributes["data-role"] = "button";
            
            _submitOptout.Click += submitOptout_Click;

            if (!IsPostBack)
            {
                _completePanel.Visible = false;

                if (!ApplicationManager.AppSettings.EnableOptOutScreen)
                    Response.Redirect(ResolveUrl(SurveyUrl));
            }
        }

        /// <summary>
        /// Submits Opt-out
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void submitOptout_Click(object sender, EventArgs e)
        {
            _greetingsPanel.Visible = false;
            _completePanel.Visible = true;

            InvitationOptOutType type;
            if (Enum.TryParse(_options.SelectedValue, out type))
                InvitationManager.OptOutRecipient(Recipient, Survey.ID, type, _userCommentTextArea.Text);
        }
    }
}

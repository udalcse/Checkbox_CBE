using System;
using System.EnterpriseServices;
using AjaxControlToolkit;
using Amazon.DynamoDBv2.DocumentModel;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    public partial class Share : Checkbox.Web.Common.UserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

      

        /// <summary>
        /// 
        /// </summary>
        public ResponseTemplate ResponseTemplate
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string SurveyURL
        {
            get
            {
                string surveyUrlWithGuid = ResolveUrl("~/Survey.aspx") + "?s=" + ResponseTemplate.GUID.ToString().Replace("-", String.Empty);
                return ApplicationManager.ApplicationURL + surveyUrlWithGuid;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SurveyCustomURL
        {
            get
            {
                return ApplicationManager.ApplicationURL + SurveyCustomURLRelative;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string SurveyCustomURLRelative
        {
            get
            {
                var surveyUrlWithGuid = ResolveUrl("~/Survey.aspx") + "?s=" + ResponseTemplate.GUID.ToString().Replace("-", String.Empty);
                return UrlMapper.GetSource(surveyUrlWithGuid);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SendCustomInvitationButton_OnClick(object sender, EventArgs e)
        {
            // need to generate unique invite strings for each case
              string pageName = Checkbox.Messaging.Email.EmailGateway.ProviderSupportsBatches ? "AddScheduled.aspx" : "Add.aspx";
             Response.Redirect(ResolveUrl(string.Format("~/Forms/Surveys/Invitations/{0}?s={1}&useCustomUrl={2}", pageName, ResponseTemplate.ID, true)));
        }

        protected void SendInvitationButton_OnClick(object sender, EventArgs e)
        {
            // need to generate unique invite strings for each case
            string pageName = Checkbox.Messaging.Email.EmailGateway.ProviderSupportsBatches ? "AddScheduled.aspx" : "Add.aspx";
            Response.Redirect(ResolveUrl(string.Format("~/Forms/Surveys/Invitations/{0}?s={1}&useCustomUrl={2}", pageName, ResponseTemplate.ID, false)));
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ResponseTemplate"></param>
        internal void Initialize(ResponseTemplate ResponseTemplate)
        {
            this.ResponseTemplate = ResponseTemplate;
            _customhtmlCode.Text = EmbededHtmlCodeCustomUrl;
            _defaultHTML.Text = EmbededHtmlCodeStandardUrl;
            _surveyCustomURL.Text = SurveyCustomURL;
            _surveyUrl.Text = SurveyURL;
    
            if (Utilities.IsNotNullOrEmpty(SurveyCustomURLRelative) &&
                ApplicationManager.AppSettings.AllowSurveyUrlRewriting)
            {
                CustomEnabled = true;
            }
            else
            {
                CustomEnabled = false;
            }




        }

        /// <summary>
        /// 
        /// </summary>
        protected string EmbededHtmlCodeStandardUrl
        {
            get { return string.Format("<iframe width=\"800\" height=\"600\"\n src=\"{0}\">\n</iframe>", SurveyURL); }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string EmbededHtmlCodeCustomUrl
        {
            get { return string.Format("<iframe width=\"800\" height=\"600\"\n src=\"{0}\">\n</iframe>", SurveyCustomURL); }
        }

        protected string CustomTwitterUrl
        {

            get
            {
                string text = string.Format("{0} {1}", WebTextManager.GetText("/twitterButton/tweetText"), ResponseTemplate.Name);
                return "https://twitter.com/intent/tweet?url=" +
                       Utilities.AdvancedHtmlEncode(SurveyCustomURL) +
                       "&text=" + Utilities.AdvancedHtmlEncode(text);

            }
        }

        protected string CustomLinkedInUrl
        {
            get
            {
                return Utilities.AdvancedHtmlEncode("http://www.linkedin.com/shareArticle?mini=true&url=" + SurveyCustomURL + "&title=" +
                       ResponseTemplate.Name);
            }
        }

        protected string CustomFacebookUrl
        {
            get
            {
                
                return Utilities.AdvancedHtmlEncode("http://www.facebook.com/sharer.php?s=100&p[url]=" + SurveyCustomURL);

            }
        }

        protected string CustomGplusUrl
        {
            get
            {
               
                return Utilities.AdvancedHtmlEncode("https://plus.google.com/share?url=" + SurveyCustomURL);
            }
        }
        protected string TwitterUrl
        {

            get
            {
                string text = string.Format("{0} {1}", WebTextManager.GetText("/twitterButton/tweetText"), ResponseTemplate.Name);
                return "https://twitter.com/intent/tweet?url=" +
                       Utilities.AdvancedHtmlEncode(SurveyURL) +
                       "&text=" + Utilities.AdvancedHtmlEncode(text);

            }
        }
        protected string LinkedInUrl
        {
            get
            {
                return Utilities.AdvancedHtmlEncode("http://www.linkedin.com/shareArticle?mini=true&url=" + SurveyURL + "&title=" +
                       ResponseTemplate.Name);
            }
        }

        protected string FacebookUrl
        {
            get
            {

                return Utilities.AdvancedHtmlEncode("http://www.facebook.com/sharer.php?s=100&p[url]=" + SurveyURL);

            }
        }

        protected string GplusUrl
        {
            get
            {

                return Utilities.AdvancedHtmlEncode("https://plus.google.com/share?url=" + SurveyURL);
            }
        }
        
        protected bool CustomEnabled { get; private set; }
       

    }
}

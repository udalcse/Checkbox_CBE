using System;
using System.Collections.Specialized;
using Checkbox.Management;
using Checkbox.Forms.Items.Configuration;
using Prezza.Framework.Common;
using Checkbox.Common;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Item representing a redirect
    /// </summary>
    [Serializable]
    public class RedirectItem : ResponseItem
    {
        private string _url;
        private int? _autoRedirectDelay;
        private bool _autoRedirect;
        private bool _restartSurvey;
        private bool _openInNewWindow;
        private string _urlText;

        /// <summary>
        /// Configure the item
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            Visible = ExportMode != ExportMode.Pdf;
            ArgumentValidation.CheckExpectedType(configuration, typeof(RedirectItemData));

            base.Configure(configuration, languageCode, templateId);

            var config = (RedirectItemData)configuration;

            _restartSurvey = config.RestartSurvey;
            _openInNewWindow = config.OpenInNewWindow;

            _url = config.RestartSurvey 
                ? string.Format("{0}/survey.aspx?s={1}ResponseTemplateGUID&ForceNew=true", ApplicationManager.ApplicationPath, ApplicationManager.AppSettings.PipePrefix) 
                : config.URL;

            _autoRedirectDelay = config.AutoRedirectDelayTime;
            _autoRedirect = config.RedirectAutomatically;

            _urlText = GetText(config.URLTextID);
        }

        /// <summary>
        /// Get the text for the link
        /// </summary>
        public virtual string LinkText
        {
            get { return GetPipedText("UrlText", _urlText); }
        }
    
        /// <summary>
        /// Get the URL for the link
        /// </summary>
        public virtual string Url
        {
            get { return GetPipedText("Url", _url).Trim(); }
        }

        /// <summary>
        /// Get whether to redirect automatically or provide a link
        /// </summary>
        public virtual bool RedirectAutomatically
        {
            get { return _autoRedirect; }
        }

        /// <summary>
        /// Get whether to redirect automatically or provide a link
        /// </summary>
        public virtual int RedirectAutomaticallyDelay
        {
            get { return _autoRedirectDelay ?? 0; }
        }

        /// <summary>
        /// Get whether the survey should be restarted
        /// </summary>
        public virtual bool RestartSurvey
        {
            get { return _restartSurvey; }
        }

        /// <summary>
        /// Get/set whether to open in the same window or tab
        /// </summary>
        public virtual bool OpenInNewWindow
        {
            get { return _openInNewWindow; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetMetaDataValuesForSerialization()
        {
            var values =  base.GetMetaDataValuesForSerialization();

            values["isRedirectAutomatic"] = RedirectAutomatically.ToString();
            values["isRestartSurvey"] = RestartSurvey.ToString();
            values["redirectAutomaticallyDelay"] = RedirectAutomaticallyDelay.ToString();
            values["isOpenInNewWindow"] = OpenInNewWindow.ToString();

            return values;
        }

        /// <summary>
        /// Get instance data for serialization
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetInstanceDataValuesForSerialization()
        {
            NameValueCollection values = base.GetInstanceDataValuesForSerialization();

            values["redirectUrl"] = Url;
            values["linkText"] = LinkText;

            return values;
        }
    }
}
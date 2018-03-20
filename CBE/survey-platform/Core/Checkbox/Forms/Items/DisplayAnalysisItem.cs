using System;
using System.Collections.Specialized;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Security;
using Checkbox.Management;
using Prezza.Framework.Common;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Survey item that displays a report item in the survey.
    /// </summary>
    [Serializable()]
    public class DisplayAnalysisItem : RedirectItem
    {
        /// <summary>
        /// Should be shown in new browser tab
        /// </summary>
        public bool ShowInNewTab { set; get; }

        /// <summary>
        /// Always return false
        /// </summary>
        public override bool RestartSurvey
        {
            get { return false; }
        }

        /// <summary>
        /// Get the URL
        /// </summary>
        public override string Url
        {
            get
            {
                try
                {
                    if (Response != null)
                    {
                        //Create a ticket in the database
                        Ticketing.CreateTicket(Response.GUID.Value, DateTime.Now.AddMinutes(ApplicationManager.AppSettings.ViewReportTicketDuration));

                        return base.Url + "&tg=" + Response.GUID;
                    }
                }
                catch
                {
                }

                return base.Url;
            }
        }

        /// <summary>
        /// Configure the item
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            ArgumentValidation.CheckExpectedType(configuration, typeof(DisplayAnalysisItemData));

            base.Configure(configuration, languageCode, templateId);

            var config = (DisplayAnalysisItemData)configuration;

            ShowInNewTab = config.ShowInNewTab;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetMetaDataValuesForSerialization()
        {
            var values = base.GetMetaDataValuesForSerialization();

            values["showInNewTab"] = ShowInNewTab.ToString();

            return values;
        }
    }
}

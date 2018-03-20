using System;
using System.Collections.Specialized;

using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using Checkbox.Forms.Items.Configuration;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Item that displays HTML.  The item also supports localizable tokens, so the most of the HTML can
    /// remain constant, with only the tokens being localized.
    /// </summary>
    [Serializable]
    public class HtmlItem : ResponseItem
    {
        private string _html;
        private string _inlineCss;

        /// <summary>
        /// Get HTML for the item
        /// </summary>
        public string Html
        {
            get
            {
                if (Response == null && ApplicationManager.AppSettings.DisplayHtmlItemsAsPlainText)
                {
                    return Utilities.StripHtml(GetPipedText("HTML", _html), null);
                }

                return GetPipedText("HTML", _html);
            }
        }

        /// <summary>
        /// Get the Inline CSS associated with the item
        /// </summary>
        public string InlineCss
        {
            get { return GetPipedText("CSS", _inlineCss);}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            base.Configure(configuration, languageCode, templateId);

            ArgumentValidation.CheckExpectedType(configuration, typeof(HtmlItemData));
            var config = (HtmlItemData)configuration;

            _html = config.HTML;
            _inlineCss = config.InlineCSS;
        }

        /// <summary>
        /// Get instance values for xml
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetInstanceDataValuesForSerialization()
        {
            NameValueCollection values = base.GetInstanceDataValuesForSerialization();

            values["InlineCss"] = InlineCss;
            values["Html"] = Html;

            return values;
        }
    }
}
